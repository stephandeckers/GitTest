- The backend is generated outside the project using batchfile scaffold.bat which is part of the project. The generated code is then picked up
  and modified :
  
o change all decimal to long
o change all TypeName = "NUMBER(38) to TypeName = "NUMBER(18)"
o change all NUMBER to NUMBER(18) 
o remove PK's from generated part to partial part to enable Swagger attributes

  public partial class RefEqPortType			: IRef_Data	{}

