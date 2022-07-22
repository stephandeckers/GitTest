@echo off
REM @Name scaffold_model.bat
REM @Author S.Deckers
REM @Date 31 May 2021
REM @Purpose Generate model classes
REM 
REM Notes:
REM - for generation to work, install the following packages 
REM  PM>Install-Package Microsoft.EntityFrameworkCore.Design
REM  PM>Install-Package Oracle.EntityFrameworkCore
REM
REM - You need to generate all models in one go, otherwise the ModelContext.cs is generated with incomplete definitions

REM Test Usage (20100301 SDE)
REM -------------------------
if "%2" == "" ( echo Usage:sc.bat -conf DEV5/DEV && goto :EOF)
if /i "%2" == "DEV5" 	(  set ORA_CREDENTIALS=ifh_owner/ZD6R_yHVPGbmcb@IFH03DEVADB	&& goto :DEV5 )
if /i "%2" == "DEV" 	(  set ORA_CREDENTIALS=ifh_owner/UnytR_OpuN1@IFH05DEVADB 	&& goto :DEV )
goto :EOF

REM DEV5 (20210310 SDE)
REM ---------------------
:DEV5
set credentials="User Id=ifh_owner;Password=UnytR_OpuN1;Data Source=IFH74D:1540/IFH74D.BC;"
set out="Model\Generated"
set ns="PXS.IfRF.Data.Model"

echo Generating for DEV5
dotnet ef dbcontext scaffold %credentials% Oracle.EntityFrameworkCore  --no-build -f -o %out% -n %ns% ^
-t IRF_CONNECTION ^
-t IRF_POP ^
-t IRF_RACK_SPACE ^
-t IRF_REF_EQ_PORT_TYPES ^
-t IRF_REF_PL_CONNECTION_STATUS ^
-t IRF_REF_PL_CONNECTION_TYPE ^
-t IRF_REF_PL_FRAME_TYPE ^
-t IRF_REF_PL_ORDER_STATUS ^
-t IRF_REF_PL_ORDER_TYPE ^
-t IRF_REF_PL_OWNER ^
-t IRF_REF_PL_POP_STATUS ^
-t IRF_REF_PL_POP_TYPE ^
-t IRF_REF_PL_POSITION_STATUS ^
-t IRF_REF_PL_POSITION_TYPE ^
-t IRF_REF_PL_SUBRACK_STATUS ^
-t IRF_REF_PL_SUBRACK_TYPE ^
-t IRF_REF_ZONES ^
-t IRF_RESOURCE ^
-t IRF_RESOURCE_CHARACTERISTIC ^
-t IRF_RESOURCE_ORDER ^
-t IRF_RF_AREA ^
-t IRF_SPEC_POP_ASSEMBLY ^
-t IRF_SPEC_POP_SUBRACK_MAP ^
-t IRF_SPEC_SR_POSITION ^
-t IRF_SPEC_SR_POS_GROUP ^
-t IRF_SPEC_SUBRACK ^
-t IRF_SR_POSITION ^
-t IRF_SUBRACK

goto :EOF

rem new env All in once
rem ---------------------------------------------------------------------------------------------------------------
:DEV
set credentials="User Id=IFRF_SCHEMA;Password=Sch4IFRF_DeV;Data Source=IFRF70D:1540/IFRF70D.BC;"
set out="Model\Generated"
set ns="PXS.IfRF.Data.Model"
echo Generating for DEV
dotnet ef dbcontext scaffold %credentials% Oracle.EntityFrameworkCore --data-annotations --no-build -f -o %out% -n %ns% ^
-t RS_RESOURCE_ORDER ^
-t RS_RESOURCE ^
-t RS_RESOURCE_CHARACTERISTIC ^
-t REF_ZONES ^
-t RS_RF_AREA ^
-t META_SPEC_POP_ASSEMBLY ^
-t RS_POP ^
-t META_SPEC_SUBRACK ^
-t META_SPEC_SR_POSITION ^
-t META_SPEC_POP_SUBRACK_MAP ^
-t META_SPEC_SR_POS_GROUP ^
-t RS_RACK_SPACE ^
-t RS_SUBRACK ^
-t RS_SR_POSITION ^
-t RS_CONNECTION ^
-t REF_PL_OWNER ^
-t REF_PL_POP_TYPE ^
-t REF_PL_POP_STATUS ^
-t REF_PL_SUBRACK_TYPE ^
-t REF_PL_SUBRACK_STATUS ^
-t REF_PL_FRAME_TYPE ^
-t REF_PL_POSITION_TYPE ^
-t REF_PL_POSITION_STATUS ^
-t REF_PL_CONNECTION_TYPE ^
-t REF_PL_CONNECTION_STATUS ^
-t REF_PL_ORDER_TYPE ^
-t REF_PL_ORDER_STATUS ^
-t REF_EQ_PORT_TYPES ^
-t META_PL_DEFAULTS