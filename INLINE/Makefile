SYS := $(shell gcc -dumpmachine)

GLOBAL_INCLUDE=-I/usr/include/ -I/usr/local/include

ifneq (, $(findstring _64, $(SYS)))
GLOBAL_LIB=-L/usr/local/lib64 -L/usr/lib64
LIBOCILIB=-lrt -lm  -lc -lnnz10 -lclntsh -locci /usr/lib64/libstdc++.so.5
LDLIBS=-L/usr/lib64 -L/lib64
else
GLOBAL_LIB=-L/usr/local/lib -L/usr/lib
LIBOCILIB=-lrt -lm  -lc -lnnz10 -lclntsh -locci /usr/lib/libstdc++.so.5
LDLIBS=-L/usr/lib -L/lib
endif

OCCI_LIB=-L${ORACLE_HOME}/rdbms/lib/ -L${ORACLE_HOME}/lib/

LOCAL_INCLUDE=-Iinclude/

ORACLEINCLIDEPATH=-I ${ORACLE_HOME}/rdbms/public/

LIBPTHREAD=-lpthread


SRC_DIR = ./src/
EXE_DIR = ./

CXX = g++
CXXCPP = g++ -E
CXXFLAGS = -g -fPIC -pipe -Wall -Werror -rdynamic -D_REENTRANT -fno-rtti

SOURCES=$(SRC_DIR)i5_1accountCvs.cpp\
	$(SRC_DIR)DBManager.cpp

TARGET=$(EXE_DIR)i5_1accountCvs

all:
	$(CXX) $(CXXFLAGS) -o $(TARGET) $(SOURCES) $(LOCAL_INCLUDE) $(GLOBAL_INCLUDE) $(ORACLEINCLIDEPATH) $(GLOBAL_LIB) $(LIBPTHREAD) $(LIBOCILIB) $(OCCI_LIB) $(LDLIBS)

clear:
	rm $(TARGET);
        
