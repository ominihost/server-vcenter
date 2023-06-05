using System;
using System.Net;
using VMware.Vim;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace OminiHost_Server
{
    public class vmwareConnect
    {
        public List<T> getEntities<T>(VimClient vimClient, ManagedObjectReference basePraBusca, NameValueCollection filter)
        {
            List<T> things = new List<T>();
            List<EntityViewBase> vBase = vimClient.FindEntityViews(typeof(T), basePraBusca, filter, null);
            if (vBase == null) return null;

            foreach (EntityViewBase eBase in vBase)
            {
                T thing = (T)(object)eBase;
                things.Add(thing);
            }
            return things;
        }
        /// <summary>
        /// Busca por uma entity e retorna somente a primeira encontrada
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vimClient"></param>
        /// <param name="baseParaBusca"></param>
        /// <param name="filter"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public T getEntity<T>(VimClient vimClient, ManagedObjectReference baseParaBusca, NameValueCollection filter)
        {
            EntityViewBase vBase = vimClient.FindEntityView(typeof(T), baseParaBusca, filter, null);          
            T thing = (T)(object)vBase;
            return thing;
        }
        /// <summary>
        /// Essa função pega um objeto, passando o moRef
        /// Exemplo: passando o moRef de uma VM ele paga todo os dados da 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vimClient"></param>
        /// <param name="moRef"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public T getObject<T>(VimClient vimClient, ManagedObjectReference moRef)
        {
            ViewBase vBase = null;
            try
            {
                vBase = vimClient.GetView(moRef, null);
            }
            catch(Exception e)
            {
                Omini.debug("ERRO em vmwareConnect.getObject \n" + e.Message);
            }
            T thisObject = (T)(object)vBase;
            return thisObject;
        }    

        public OptionValue[] addExtraConfig(OptionValue[] vmExtraConfig, string key, object value)
        {
            int len = vmExtraConfig.Length;
            OptionValue[] newExtraConfig = new OptionValue[len + 1];

            for (int i=0; i < len; i++)
            {
                OptionValue option = new OptionValue();
                option = vmExtraConfig[i];

                if(option != null)
                {
                    if (option.Key.Equals(key))
                    {
                        vmExtraConfig[i].Value = value;
                        return vmExtraConfig;
                    }
                }

                newExtraConfig[i] = option;
            }            

            newExtraConfig[len - 1] = new OptionValue
            {
                Key = key,
                Value = value
            };
            return newExtraConfig;
        }

        public int getVncPort(VirtualMachine vm)
        {
            int len = vm.Config.ExtraConfig.Length;
            for(int i=0; i < len; i++)
            {
                OptionValue option = new OptionValue();
                option = vm.Config.ExtraConfig[i];
                if(option.Key.Equals("RemoteDisplay.vnc.port"))
                {
                    return int.Parse(option.Value.ToString());
                }
            }
            return -1;
        }
        /// <summary>
        /// Pega o ResourcePool de um host(Esxi)
        /// </summary>
        /// <param name="vimClient"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        public ResourcePool getResourcePoolInHost(VimClient vimClient, HostSystem host)
        {
            ComputeResource cr = this.getObject<ComputeResource>(vimClient, host.Parent);
            return this.getObject<ResourcePool>(vimClient, cr.ResourcePool);
        }
        public HostSystem getHost(VimClient vimClient, ManagedObjectReference moRef)
        {
            return this.getObject<HostSystem>(vimClient, moRef);
        }
        /// <summary>
        /// Pega host(Esxi) que está usando menos memoria ram em um dedicado
        /// </summary>
        /// <param name="vimClient"></param>
        /// <param name="dc"></param>
        /// <returns></returns>
        public HostSystem getHostAnyLessInDatacenter(VimClient vimClient, Datacenter dc)
        {
            List<HostSystem> hosts = this.getEntities<HostSystem>(vimClient, dc.MoRef, null);
            if (hosts != null && hosts.Count > 0)
            {
                long menorUso = 999999999999;
                HostSystem usarEsseHost = null;
                foreach (HostSystem hostLoop in hosts)
                {
                    ResourcePool pool = this.getResourcePoolInHost(vimClient, hostLoop); 
                                       
                    if (menorUso > pool.Runtime.Memory.OverallUsage)
                    {
                        menorUso = pool.Runtime.Memory.OverallUsage;
                        usarEsseHost = hostLoop;
                    }
                }
                return usarEsseHost;
            }
            else return null;
        }

        public List<Dictionary<string, string>> getHostsInDataCenter(VimClient vimClient, string dcName)
        {
            Datacenter datacenter = findDatacerterByName(vimClient, dcName);
            if (datacenter != null)
            {
                List<HostSystem> hosts = this.getEntities<HostSystem>(vimClient, datacenter.MoRef, null);
                if (hosts != null && hosts.Count > 0)
                {
                    List<Dictionary<string, string>> saida = new List<Dictionary<string, string>>();
                    foreach (HostSystem host in hosts)
                    {
                        ResourcePool pool = this.getResourcePoolInHost(vimClient, host);
                        Dictionary<string, string> hostinfos = new Dictionary<string, string>();
                        hostinfos.Add("Nome", host.Name);
                        hostinfos.Add("Ram_Usando", pool.Runtime.Memory.OverallUsage.ToString());
                        hostinfos.Add("Cpu_Usando", pool.Runtime.Cpu.OverallUsage.ToString());

                        hostinfos.Add("Cpu_Total", (host.Summary.Hardware.CpuMhz * host.Summary.Hardware.NumCpuCores).ToString());
                        hostinfos.Add("Ram_Total", host.Summary.Hardware.MemorySize.ToString());
                        saida.Add(hostinfos);
                    }                    
                    return saida;
                }
                return null;
            }
            return null;
        }

        /// <summary>
        /// Pega o HD mais fazio de um host(Esxi)
        /// </summary>
        /// <param name="vimClient"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        public Datastore getDatastoreAnyLessInHost(VimClient vimClient, HostSystem host, string hdType)
        {            
            if (host != null && host.Datastore.Length > 0)
            {
                long maisVazio = 0;
                Datastore mRef = null;
                foreach(ManagedObjectReference storeRef in host.Datastore)
                {
                    Datastore store = this.getObject<Datastore>(vimClient, storeRef);

                    if(hdType.Equals("all") || store.Name.IndexOf(hdType) != -1)
                    {
                        if (store.Summary.FreeSpace > maisVazio && store.Summary.FreeSpace > 30000000000)
                        {
                            maisVazio = store.Summary.FreeSpace;
                            mRef = store;
                        } 
                    }
                }
                return mRef;
            }
            else return null;
        }
        /// <summary>
        /// procura um Template em um datacenter(vCenter), retorna o primeiro encontrada
        /// </summary>
        /// <param name="vimClient"></param>
        /// <param name="dc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public VirtualMachine findTemplateInDatacenter(VimClient vimClient, Datacenter dc,string name)
        {
            NameValueCollection filtro = new NameValueCollection();
            filtro.Add("name", name);
            filtro.Add("Config.Template", "true");
            return this.getEntity<VirtualMachine>(vimClient, dc.MoRef, filtro);
        }
        public VirtualMachine getMachine(VimClient vimClient, string name)
        {
            NameValueCollection filtro = new NameValueCollection();
            filtro.Add("name", name);
            Debug.WriteLine(filtro.ToString());
            Debug.WriteLine(vimClient.ToString());
            
            List<VirtualMachine> vmList = this.getEntities<VirtualMachine>(vimClient, null, filtro);
            //return vmList;
            Debug.WriteLine("ERRO vmlist: " + vmList);
            if (vmList == null) return null;

            foreach (VirtualMachine vm in vmList)
            {
                try
                {
                    if (vm == null)
                    {
                        continue;
                    }

                    //Regex regex = new Regex(@"\d{1,3}(\.\d{1,3}){3}");
                    Regex regexipv4 = new Regex(@"\d{1,3}(\.\d{1,3}){3}");
                    Regex regexipv6 = new Regex(@"\d([0-9a-fA-F]{1,4}:){7,7}[0-9a-fA-F]{1,4}");

                    Match matchipv6 = regexipv6.Match(vm.Name);
                    Match matchipv4 = regexipv4.Match(vm.Name);

                    bool matchresult = matchipv6.Success == true ? matchipv6.Success : matchipv4.Success;

                    if (matchresult)
                    {
                        string nameFind = matchipv6.Success == true ? matchipv6.Value.ToLower() : matchipv4.Value.ToLower();

                        if (nameFind.Equals(name.ToLower()))
                        {
                            return vm;
                        }
                    }
                    else
                        continue;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("ERRO GetMachine: " + ex.Message);
                    return null;
                }
            }
            return null;
        }
        /// <summary>
        /// Procura um dedicado pelo nome, retorna o primeiro que encontrar
        /// </summary>
        /// <param name="vimClient"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Datacenter findDatacerterByName(VimClient vimClient, string name)
        {
            NameValueCollection filtro = new NameValueCollection();
            filtro.Add("name", name);
            return this.getEntity<Datacenter>(vimClient, null, filtro);
        }
        /// <summary>
        /// Pegar um dedicado apartir do nome
        /// </summary>
        /// <param name="vimClient"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public HostSystem findDedicadoByName(VimClient vimClient, string name)
        {
            NameValueCollection filtro = new NameValueCollection();
            filtro.Add("name", name);
            return this.getEntity<HostSystem>(vimClient, null, filtro);
        }
        /// <summary>
        /// Pega o adpatador de rede E1000 de uma maquiona
        /// Pega apenas E1000
        /// </summary>
        /// <param name="CloneVMTarget"></param>
        /// <returns></returns>
        public VirtualDevice getNewNetWorkDevice(VirtualMachine CloneVMTarget)
        {
            VirtualDevice NetworkCard = new VirtualDevice();
            foreach (VirtualDevice vDevice in CloneVMTarget.Config.Hardware.Device)
            {              
                if (vDevice.Backing != null)
                {                    
                    if (vDevice.GetType().Name == "VirtualE1000")
                    {
                        NetworkCard = vDevice;                        
                    }
                }
            }
            return NetworkCard;
        }
        /// <summary>
        /// Pega o disco "fisico" de um template    
        /// </summary>
        /// <param name="CloneVMTarget"></param>
        /// <returns></returns>
        public VirtualDevice getDiskDeviceClone(VirtualMachine CloneVMTarget)
        {
            VirtualDevice diskDevice = new VirtualDevice();
            foreach (VirtualDevice vDevice in CloneVMTarget.Config.Hardware.Device)
            {
                if (vDevice.Backing != null)
                {
                    if (vDevice.GetType().Name == "VirtualDevice")
                    {
                        diskDevice = vDevice;
                    }
                }
            }
            return diskDevice;
        }
        /// <summary>
        /// Pega o disco "fisco" de uma maquina 
        /// Maquina passada via moRef
        /// </summary>
        /// <param name="vimClient"></param>
        /// <param name="moref"></param>
        /// <returns></returns>
        public VirtualDevice getDeviceDiskVM(VimClient vimClient, ManagedObjectReference moref)
        {
            //System.Diagnostics.Debug.WriteLine("getDeviceDiskVM iniciado");
            VirtualMachine vm = this.getObject<VirtualMachine>(vimClient, moref);
            VirtualDevice disk = new VirtualDevice();
            //System.Diagnostics.Debug.WriteLine("getDeviceDiskVM 001");
            foreach (VirtualDevice vDevice in vm.Config.Hardware.Device)
            {
                //System.Diagnostics.Debug.WriteLine("getDeviceDiskVM > Forech");
                if (vDevice.Backing != null)
                {
                    if (vDevice.GetType().Name == "VirtualDisk")
                    {
                        //System.Diagnostics.Debug.WriteLine("getDeviceDiskVM > VirtualDisk encontrado");
                        disk = vDevice;                        
                    }
                }
            }
            return disk;
        }
        /// <summary>
        /// Pega todos os HDs de uma maquina
        /// </summary>
        /// <param name="vimClient"></param>
        /// <param name="vm"></param>
        /// <returns></returns>
        public List<VirtualDevice> getAllDiskVM(VimClient vimClient, VirtualMachine vm)
        {
            List<VirtualDevice> disks = new List<VirtualDevice>();
            foreach (VirtualDevice vDevice in vm.Config.Hardware.Device)
            {
                if (vDevice.Backing != null)
                {
                    if (vDevice.GetType().Name == "VirtualDisk")
                    {
                        disks.Add(vDevice);
                    }
                }
            }
            return disks;
        }
        public long startProcessInVm(VimClient vimClient, VirtualMachine vm, string username, string password, string processPath, string arguments)
        {
            NamePasswordAuthentication auth = new NamePasswordAuthentication
            {
                InteractiveSession = false,
                Username = username,
                Password = password,
            };

            GuestProgramSpec programSpec = new GuestProgramSpec
            {
                ProgramPath = processPath,
                Arguments = arguments
            };

            GuestOperationsManager guestOperation = this.getObject<GuestOperationsManager>(vimClient, vimClient.ServiceContent.GuestOperationsManager);
            GuestProcessManager processManager = this.getObject<GuestProcessManager>(vimClient, guestOperation.ProcessManager);
            try
            {
                long pid = processManager.StartProgramInGuest(vm.MoRef, auth, programSpec);
                return pid;
            }
            catch (Exception)
            {
                Debug.WriteLine("[vmwareConnect] startProcessInVm ");
                return default;
            }
        }
        /// <summary>
        /// Pega um campo de anotação de um host ESXI
        /// </summary>
        /// <param name="host"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public string getCustomFieldInHost(HostSystem host, string keyName)
        {
            int c = -1;
            foreach(CustomFieldDef field in host.AvailableField)
            {
                c++;
               // System.Diagnostics.Debug.WriteLine("C = " + c + ", nome: " + field.Name + "Field KEY: "+ field.Key+" BATATA = "+host.CustomValue.Length);
                if (field.Name.Equals(keyName) == true)
                {                   
                    return ((CustomFieldStringValue)host.CustomValue[c]).Value;                                       
                }
            }
            return null;
        }
        /// <summary>
        /// Abre o Customization Manager e paga a custom referente ao template clone name
        /// </summary>
        /// <param name="vimClient"></param>
        /// <param name="vmPHPEspec"></param>
        /// <returns></returns>
        CustomizationSpec getWindowsCustomization(VimClient vimClient, Data.VMEspec vmPHPEspec)
        {
            CustomizationSpecManager manager = new CustomizationSpecManager(vimClient, new ManagedObjectReference("CustomizationSpecManager-CustomizationSpecManager"));
            CustomizationSpecItem custom = manager.GetCustomizationSpec(vmPHPEspec.templateSystemName);                            
           return custom.Spec;
        }
        /// <summary>
        /// Clonar uma vm
        /// </summary>
        /// <param name="vimClient"></param>
        /// <param name="vmTemplate"></param>
        /// <param name="newStore"></param>
        /// <param name="host"></param>
        /// <param name="vmFolder"></param>
        /// <param name="vmPHPEspec"></param>
        /// <returns></returns>
        public Task CloneVM(VimClient vimClient,VirtualMachine vmTemplate,Datastore newStore,HostSystem host,ManagedObjectReference vmFolder, Data.VMEspec vmPHPEspec)
        {

            string newNomeVm = vmPHPEspec.ipVM + " " + vmPHPEspec.nomeVM;
            if (!vmPHPEspec.HDType.Equals("all"))
            {
                newNomeVm = "[" + vmPHPEspec.HDType + "] " + vmPHPEspec.ipVM + " " + vmPHPEspec.nomeVM;
            }

            VirtualMachineCloneSpec newVMSpec = new VirtualMachineCloneSpec();
                                          
            newVMSpec.Config = new VirtualMachineConfigSpec();
            newVMSpec.Config.Name = newNomeVm;
            newVMSpec.Config.MemoryMB = vmPHPEspec.memoriaRam;
            newVMSpec.Config.NumCPUs = vmPHPEspec.vCPU;
            newVMSpec.Config.NumCoresPerSocket = vmPHPEspec.vCPU * vmPHPEspec.fCPU; //Processadores fisicos (NumCPUs*NumCoresPerSocket) 
            newVMSpec.Config.CpuAllocation = new ResourceAllocationInfo();
            newVMSpec.Config.CpuAllocation.Limit = vmPHPEspec.CPU_Mhz;
            newVMSpec.Config.MemoryAllocation = new ResourceAllocationInfo();
            newVMSpec.Config.MemoryAllocation.Limit = newVMSpec.Config.MemoryMB;
            

            Debug.WriteLine("SET limit VM, CPU = "+ newVMSpec.Config.CpuAllocation.Limit+" RAM = "+ newVMSpec.Config.MemoryAllocation.Limit);

            if (vmPHPEspec.sistemaOperacional.Equals("windows") == true)
            {
                newVMSpec.Customization = getWindowsCustomization(vimClient, vmPHPEspec);
                ((CustomizationSysprep)newVMSpec.Customization.Identity).GuiUnattended.Password = new CustomizationPassword { PlainText = true, Value = vmPHPEspec.novaSenha };
                
                string ipv4 = vmPHPEspec.IPv4;
                string ipv6 = vmPHPEspec.IPv6;
                string nulo = "null";

                if (ipv4 != nulo)
                {
                    //newVMSpec.Customization.NicSettingMap[0].Adapter.Gateway = new string[] { host.Config.Network.IpRouteConfig.DefaultGateway };
                    //newVMSpec.Customization.NicSettingMap[0].Adapter.Gateway = new string[] { "10.15.15.1" };
                    newVMSpec.Customization.NicSettingMap[0].Adapter.Gateway = new string[] { vmPHPEspec.IPv4Gateway };
                    newVMSpec.Customization.NicSettingMap[0].Adapter.Ip = new CustomizationFixedIp { IpAddress = vmPHPEspec.IPv4 };
                    newVMSpec.Customization.NicSettingMap[0].Adapter.SubnetMask = vmPHPEspec.IPv4Mask;
                }

                if (ipv6 != nulo)
                {
                    
                    newVMSpec.Customization.NicSettingMap[0].Adapter.IpV6Spec.Gateway = new string[] { vmPHPEspec.IPv6Gateway };
                    newVMSpec.Customization.NicSettingMap[0].Adapter.IpV6Spec.Ip = new CustomizationIpV6Generator[] { new CustomizationFixedIpV6 { IpAddress = vmPHPEspec.IPv6, SubnetMask = vmPHPEspec.IPv6Mask } };
                }

                newVMSpec.Customization.NicSettingMap[0].Adapter.DnsServerList = new string[] {
                    vmPHPEspec.IPv4DNS1, 
                    vmPHPEspec.IPv4DNS2, 
                    vmPHPEspec.IPv4DNS3, 
                    vmPHPEspec.IPv4DNS4, 
                    vmPHPEspec.IPv6DNS1, 
                    vmPHPEspec.IPv6DNS2, 
                    vmPHPEspec.IPv6DNS3, 
                    vmPHPEspec.IPv6DNS4
                };
            }


            VirtualDevice template = getNewNetWorkDevice(vmTemplate);
            if(template == null)
            {
                Debug.WriteLine("Falha ao tentar encontrar o adpatardor de rede do template: "+vmPHPEspec.templateSystemName);
                return null;
            }

            Debug.WriteLine("vmnetwork: " + vmPHPEspec.NetworkName);
            newVMSpec.Config.DeviceChange = new VirtualDeviceConfigSpec[2];
            newVMSpec.Config.DeviceChange[0] = new VirtualDeviceConfigSpec();
            newVMSpec.Config.DeviceChange[0].Operation = VirtualDeviceConfigSpecOperation.edit;
            newVMSpec.Config.DeviceChange[0].Device = new VirtualE1000();
            newVMSpec.Config.DeviceChange[0].Device.Key = template.Key;
            newVMSpec.Config.DeviceChange[0].Device.DeviceInfo = new Description();
            newVMSpec.Config.DeviceChange[0].Device.DeviceInfo.Label = template.DeviceInfo.Label; 
            newVMSpec.Config.DeviceChange[0].Device.DeviceInfo.Summary = template.DeviceInfo.Summary; 
            newVMSpec.Config.DeviceChange[0].Device.Backing = new VirtualEthernetCardNetworkBackingInfo();
            //((VirtualEthernetCardNetworkBackingInfo)newVMSpec.Config.DeviceChange[0].Device.Backing).DeviceName = ((VirtualEthernetCardNetworkBackingInfo)template.Backing).DeviceName; 
            ((VirtualEthernetCardNetworkBackingInfo)newVMSpec.Config.DeviceChange[0].Device.Backing).DeviceName = vmPHPEspec.NetworkName;
            ((VirtualEthernetCardNetworkBackingInfo)newVMSpec.Config.DeviceChange[0].Device.Backing).UseAutoDetect = false;
            newVMSpec.Config.DeviceChange[0].Device.Connectable = new VirtualDeviceConnectInfo();
            newVMSpec.Config.DeviceChange[0].Device.Connectable.StartConnected = template.Connectable.StartConnected;
            newVMSpec.Config.DeviceChange[0].Device.Connectable.AllowGuestControl = template.Connectable.AllowGuestControl; 
            newVMSpec.Config.DeviceChange[0].Device.Connectable.Connected = template.Connectable.Connected; 
            newVMSpec.Config.DeviceChange[0].Device.Connectable.Status = template.Connectable.Status; 
            newVMSpec.Config.DeviceChange[0].Device.ControllerKey = template.ControllerKey;
            newVMSpec.Config.DeviceChange[0].Device.UnitNumber = template.UnitNumber;
            ((VirtualE1000)newVMSpec.Config.DeviceChange[0].Device).AddressType = ((VirtualE1000)template).AddressType;

            if (vmPHPEspec.macAdress.Equals("null") == true)
            {             
                ((VirtualE1000)newVMSpec.Config.DeviceChange[0].Device).MacAddress = ((VirtualE1000)template).MacAddress;
            }
            else
            {                
                ((VirtualE1000)newVMSpec.Config.DeviceChange[0].Device).MacAddress = vmPHPEspec.macAdress;                
            }

            ((VirtualE1000)newVMSpec.Config.DeviceChange[0].Device).WakeOnLanEnabled = ((VirtualE1000)template).WakeOnLanEnabled;            

            VirtualDevice templateDisk = this.getDeviceDiskVM(vimClient, vmTemplate.MoRef);
            if (templateDisk == null)
            {
                Debug.WriteLine("Falha ao tentar encontrar o disco do template: " + vmPHPEspec.templateSystemName);
                return null;
            }

            newVMSpec.Config.DeviceChange[1] = new VirtualDeviceConfigSpec();
            newVMSpec.Config.DeviceChange[1].Operation = VirtualDeviceConfigSpecOperation.edit;
            newVMSpec.Config.DeviceChange[1].Device = new VirtualDisk();
            newVMSpec.Config.DeviceChange[1].Device.Key = templateDisk.Key;
            newVMSpec.Config.DeviceChange[1].Device.DeviceInfo = new Description();
            newVMSpec.Config.DeviceChange[1].Device.DeviceInfo.Label = templateDisk.DeviceInfo.Label;
            newVMSpec.Config.DeviceChange[1].Device.DeviceInfo.Summary = templateDisk.DeviceInfo.Summary;
            newVMSpec.Config.DeviceChange[1].Device.Backing = (VirtualDiskFlatVer2BackingInfo)templateDisk.Backing;            
            newVMSpec.Config.DeviceChange[1].Device.ControllerKey = templateDisk.ControllerKey;
            newVMSpec.Config.DeviceChange[1].Device.UnitNumber = templateDisk.UnitNumber;
            ((VirtualDisk)newVMSpec.Config.DeviceChange[1].Device).CapacityInKB = vmPHPEspec.HDinKB+(2000000);
            ((VirtualDisk)newVMSpec.Config.DeviceChange[1].Device).Shares = new SharesInfo();
            ((VirtualDisk)newVMSpec.Config.DeviceChange[1].Device).Shares.Shares = ((VirtualDisk)templateDisk).Shares.Shares;
            ((VirtualDisk)newVMSpec.Config.DeviceChange[1].Device).Shares.Level = SharesLevel.normal;
            ((VirtualDisk)newVMSpec.Config.DeviceChange[1].Device).StorageIOAllocation = new StorageIOAllocationInfo();
            ((VirtualDisk)newVMSpec.Config.DeviceChange[1].Device).StorageIOAllocation.Limit = ((VirtualDisk)templateDisk).StorageIOAllocation.Limit;
            ((VirtualDisk)newVMSpec.Config.DeviceChange[1].Device).StorageIOAllocation.Shares = new SharesInfo();
            ((VirtualDisk)newVMSpec.Config.DeviceChange[1].Device).StorageIOAllocation.Shares.Shares = ((VirtualDisk)templateDisk).StorageIOAllocation.Shares.Shares;
            ((VirtualDisk)newVMSpec.Config.DeviceChange[1].Device).StorageIOAllocation.Shares.Level = SharesLevel.normal;

            newVMSpec.Location = new VirtualMachineRelocateSpec();
            newVMSpec.Location.Datastore = newStore.MoRef;
            newVMSpec.Location.Pool = getResourcePoolInHost(vimClient, host).MoRef;            
            newVMSpec.Template = false;  
            newVMSpec.PowerOn = true;                    
            //newVMSpec.   
                          
            ManagedObjectReference CloneTaskMoRef = new ManagedObjectReference();
            
            CloneTaskMoRef = vmTemplate.CloneVM_Task(vmFolder, newNomeVm, newVMSpec);
            Task CloneTask = new Task(vimClient, CloneTaskMoRef);
           
            return CloneTask;
        }
        public bool sessionAlive(VimClient vimClient)
        {
            SessionManager sessioManager = this.getObject<SessionManager>(vimClient, vimClient.ServiceContent.SessionManager);
            if (sessioManager.SessionIsActive(sessioManager.CurrentSession.Key, sessioManager.CurrentSession.UserName))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Essa função conecta ao vmware  
        /// </summary>
        /// <param name="viServer"></param>
        /// <param name="Credential"></param>
        /// <returns>Retorna um object VimClient</returns>
        public VimClient ConnectServer(string viServer, NetworkCredential Credential)
        {     
            VimClient vimClient = new VimClient();
            ServiceContent vimServiceContent = new ServiceContent();
            UserSession vimSession = new UserSession();                        
            string viUser = Credential.UserName;
            string viPassword = Credential.Password;
           
            try
            {
                vimClient.Connect(ValidateServer(viServer));
                vimSession = vimClient.Login(viUser, viPassword);
                vimServiceContent = vimClient.ServiceContent;
                return vimClient;
            }
            catch(Exception e)
            {
                Debug.WriteLine("[VMWare] ERRO Connect", e.Message);
                                              
                return null;
            }
        }
        /// <summary>
        /// Essa função disconnecta de um vmware
        /// </summary>
        /// <param name="vimClient"></param>
        /// <returns>Retorna true se desconectou ou false se houve errors</returns>
        public bool DisconnectServer(VimClient vimClient)
        {
            try
            {
                vimClient.Disconnect();
            }
            catch (Exception e)
            {                
                Debug.WriteLine("[VMWare] ERRO Disconnect", e.Message,false);
                
                return false;
            }
            return true;
        }
        /// <summary>
        /// Essa função formata um ip pra um link vmware sdk válido
        /// </summary>
        /// <param name="viServer"></param>
        /// <returns>Retorna string URL valida</returns>
        private static string ValidateServer(string viServer)
        {            
            viServer = viServer.Trim().ToLower();
            if (viServer.Contains("://") == false)                        
                viServer = "https://" + viServer;
                        
            Uri uriVServer = new Uri(viServer);           
            string urlScheme = uriVServer.Scheme;
            switch (urlScheme)
            {
                case "https": break;
                default:
                    viServer = viServer.Replace(uriVServer.Scheme + "://", "https://");
                break;
            }
          
            if (uriVServer.AbsolutePath == "/")
            {
                viServer = viServer + "/sdk";
            }
            else if (uriVServer.AbsolutePath != "/sdk")
            {               
                viServer = viServer.Replace(uriVServer.AbsolutePath, "/sdk");
            }
            return viServer;
        }            
    }
}
