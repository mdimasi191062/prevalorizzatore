CLASSPATH=$JAVA_HOME/lib/rt.jar:$JAVA_HOME/lib/tools.jar:$JAVA_HOME/jre/lib/rt.jar:./JRibesCommander.jar:$ORACLE_HOME/jdbc/lib/classes12.jar

export CLASSPATH

##############$JAVA_HOME/bin/java -Xms32m -Xmx64m -d64 com.ds.jribes.JRibes $1
CURR_DIR=`pwd`
cd $HOME/JribesCommander
# impostare per la variabile PROG_NAME il nome del processo descritto sulla tabella ANAG_SISTEMI
$JAVA_HOME/bin/java jribescommander.JRibesCommander $@ 
cd $CURR_DIR



