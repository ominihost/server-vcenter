@ECHO OFF
vmrun\vmrun -T ominihost1 -h https://%1/sdk -u root -p %2 -gu root -gp omini123* runProgramInGuest %3 /usr/bin/ipconf %4 %5 %6 >> logs\vmrum.log
exit