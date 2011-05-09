@echo off

copy TypeEmitAdministrator\bin\Debug\CustomModels*.* TypeEmitConsumer\bin\Debug\
copy TypeEmitAdministrator\bin\Release\CustomModels*.* TypeEmitConsumer\bin\Release\

echo Deployed...
pause