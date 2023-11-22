echo INIZIO SCANSIONE JPUB Jribes Commander

set classpath=C:\TELECOM-PROGETTO\JRIBES_COMMANDER

echo Clean del buildId
sourceanalyzer -64 -Xmx4096M -b JribesCommander -clean

echo JRIBES 
sourceanalyzer -64 -Xmx4096M -b JribesCommander -source 1.5 -cp "%classpath%\lib\ojdbc14.jar" "%classpath%\src\jribescommander\**\*.java" -logfile "C:\TELECOM-PROGETTO\JRIBES_COMMANDER\JribesCommander.log"

echo SCANSIONE
sourceanalyzer -64 -Xmx4096M -b JribesCommander -machine-output -format fpr -source 1.5 -scan -f "C:\TELECOM-PROGETTO\JRIBES_COMMANDER\JPUB_RIBES_COMMANDER.fpr" -logfile "C:\TELECOM-PROGETTO\JRIBES_COMMANDER\JribesCommander.log"

echo FINE SCANSIONE Jribes Commander
