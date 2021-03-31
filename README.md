# StackQL
Call SQL commands programmatically

  using (MySqlConnection sqlConnection = new MySqlConnection("..."))  // TODO: fill db connection arguments
  {
      sqlConnection.Open();

      /*
       TODO Create DemoDB

          CREATE TABLE IF NOT EXISTS `DEMO` (
            `Id` char(24) NOT NULL,
            `Name` varchar(128),
            `ZipCode` int(11),
            `CreateTime` datetime,
            PRIMARY KEY (`Id`)
          );
       */

      // SELECT * FROM Demo;
      Db.Query(DemoDB.TABLE_DEMO)
          .QueryTable(sqlConnection);

      // SELECT Id,Name FROM Demo;
      Db.Query(DemoDB.TABLE_DEMO)
          .Select(
              DemoDB.COL_ID,
              DemoDB.COL_NAME
          )
          .QueryTable(sqlConnection);

      // SELECT Id,Name FROM Demo WHERE Id='123';
      Db.Query(DemoDB.TABLE_DEMO)
          .Select(
              DemoDB.COL_ID,
              DemoDB.COL_NAME
          )
          .Where(
              Db.EqColEqVal(DemoDB.COL_ID, "123")     // Id = '123'
          )
          .QueryTable(sqlConnection);

      // SELECT Id,Name FROM Demo WHERE Id>'123' AND Id<'456';
      Db.Query(DemoDB.TABLE_DEMO)
          .Select(
              DemoDB.COL_ID,
              DemoDB.COL_NAME
          )
          .Where(
              Db.EqAnds(  // AND
                  Db.Eq(DemoDB.COL_ID, DbEquation.Op.Gt, "123"),  // Id > '123'
                  Db.Eq(DemoDB.COL_ID, DbEquation.Op.Lt, "456")   // Id < '456'
              )
          )
          .QueryTable(sqlConnection);

      // SELECT Id,Name FROM Demo WHERE (Id>'123' AND Id<'456') OR (Name LIKE 'ABC%');
      Db.Query(DemoDB.TABLE_DEMO)
          .Select(
              DemoDB.COL_ID,
              DemoDB.COL_NAME
          )
          .Where(
              Db.EqOrs(   // OR
                  Db.EqAnds(  // AND
                      Db.Eq(DemoDB.COL_ID, DbEquation.Op.Gt, "123"),  // Id > '123'
                      Db.Eq(DemoDB.COL_ID, DbEquation.Op.Lt, "456")   // Id < '456'
                  ),
                  Db.Eq(DemoDB.COL_NAME, DbEquation.Op.Like, "ABC%")      // Name LIKE 'ABC%'
              )
          )
          .QueryTable(sqlConnection);

      // SELECT Id,Name FROM Demo WHERE Id IN ("123", "456");
      Db.Query(DemoDB.TABLE_DEMO)
          .Select(
              DemoDB.COL_ID,
              DemoDB.COL_NAME
          )
          .Where(
              Db.Eq(
                  DemoDB.COL_ID,      // Id
                  DbEquation.Op.In,   // IN
                  new string[] {      // ("123", "456")
                      "123", 
                      "456" 
                  }
              )
          )
          .QueryTable(sqlConnection);

      // SELECT Id,Name FROM Demo WHERE Id IN (SELECT Id FROM Demo WHERE ZipCode="200");
      Db.Query(DemoDB.TABLE_DEMO)
          .Select(
              DemoDB.COL_ID,
              DemoDB.COL_NAME
          )
          .Where(
              Db.Eq(
                  DemoDB.COL_ID,      // Id
                  DbEquation.Op.In,   // IN
                  Db.Query(DemoDB.TABLE_DEMO)     // (SELECT Id FROM Demo WHERE ZipCode="200")
                      .Select(DemoDB.COL_ID)
                      .Where(
                          Db.EqColEqVal(DemoDB.COL_ZIP_CODE, "200"
                      )
                  )
              )
          )
          .QueryTable(sqlConnection);

      // SELECT Id,Name FROM Demo WHERE Id>'123' ORDER BY ZipCode, Name DESC;
      Db.Query(DemoDB.TABLE_DEMO)
          .Select(
              DemoDB.COL_ID,
              DemoDB.COL_NAME
          )
          .Where(
              Db.Eq(DemoDB.COL_ID, DbEquation.Op.Gt, "123")     // Id > '123'
          )
          .OrderBy(       // Order By
              DemoDB.COL_ZIP_CODE,        // ZipCode
              DemoDB.COL_NAME.DESC        // Name DESC
          )
          .QueryTable(sqlConnection);

      // Insert DEMO Id,Name,ZipCode,CreateTime VALUES("789", "HELLO", "201", NOW());
      Db.Insert(DemoDB.TABLE_DEMO)
          .ColumnValue(DemoDB.COL_ID, "789")
          .ColumnValue(DemoDB.COL_NAME, "HELLO")
          .ColumnValue(DemoDB.COL_ZIP_CODE, "201")
          .ColumnValue(DemoDB.COL_CREATE_TIME, Db.Func("NOW"))    // Eq. DbDateTime.SERVER_NOW
          .Insert(sqlConnection);

      // Transaction
      MySqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
      try
      {
          // Update
          Db.Update(DemoDB.TABLE_DEMO)
              .Set(DemoDB.COL_NAME, "HELLO2")
              .Where(
                  Db.EqColEqVal(DemoDB.COL_ID, "789")
              )
              .Update(sqlConnection, sqlTransaction);

          // TODO:
          throw new Exception("make some exception");

          // Commit
          sqlTransaction.Commit();
      }
      catch (Exception)
      {
          // Rollback
          sqlTransaction.Rollback();
      }

      // Optional WHERE conditions
      string id = "123";
      string name = null;
      Db.Query(DemoDB.TABLE_DEMO)
          .Select(
              DemoDB.COL_ID,
              DemoDB.COL_NAME
          )
          .Where(
              Db.EqAnds(  // AND
                  Db.EqColEqVal(DemoDB.COL_ID, id, id != null),       // Id = id (if id not null)
                  Db.EqColEqVal(DemoDB.COL_NAME, name, name != null)  // Name = name (if name not null)
              )
          )
          .QueryTable(sqlConnection);

      sqlConnection.Close();
  }
