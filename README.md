# StackQL
Call SQL commands programmatically

## Basic usage
```csharp
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

// Define TABLE schema
public class DemoDB {
  static public readonly string TABLE_DEMO = "DEMO";
  static public readonly DbColumn COL_ID = Db.Col("Id");
  static public readonly DbColumn COL_NAME = Db.Col("Name");
  static public readonly DbColumn COL_ZIP_CODE = Db.Col("ZipCode");
  static public readonly DbColumn COL_CREATE_TIME = Db.Col("CreateTime");
}
```

```csharp
using (MySqlConnection sqlConnection = new MySqlConnection(/*db connection string*/))
{
  sqlConnection.Open();

  // Do Query/Insert/Update/Delete here
  Db.Query(DemoDB.TABLE_DEMO)
      .QueryTable(sqlConnection);

  sqlConnection.Close();
}
```

## Query
```csharp
// SELECT Id,Name FROM Demo;
Db.Query(DemoDB.TABLE_DEMO)
  .Select(
      DemoDB.COL_ID,
      DemoDB.COL_NAME
  )
  .QueryTable(sqlConnection);
```

```csharp
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
```

```csharp
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
```

## Insert
```csharp
// Insert DEMO Id,Name,ZipCode,CreateTime VALUES("789", "HELLO", "201", NOW());
Db.Insert(DemoDB.TABLE_DEMO)
  .ColumnValue(DemoDB.COL_ID, "789")
  .ColumnValue(DemoDB.COL_NAME, "HELLO")
  .ColumnValue(DemoDB.COL_ZIP_CODE, "201")
  .ColumnValue(DemoDB.COL_CREATE_TIME, Db.Func("NOW"))    // Eq. DbDateTime.SERVER_NOW
  .Insert(sqlConnection);
```

## Transaction & Update
```csharp
// Transaction
MySqlTransaction sqlTransaction = sqlConnection.BeginTransaction();
try
{
  // Update DEMO SET Name="HELLO2" WHERE Id="789";
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
```

## Delete
```csharp
// Delete FROM DEMO WHERE Id="789";
Db.Delete(DemoDB.TABLE_DEMO)
    .Where(
        Db.EqColEqVal(DemoDB.COL_ID, "789")
    )
    .Delete(sqlConnection);
```

## WHERE conditions
```csharp
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
```

```csharp
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
```

```csharp
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
```

```csharp
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
```

## Optional WHERE conditions
```csharp
string id = "123";    // TODO: input id
string name = null;    // TODO: input name
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
```

## Cache
```csharp
// Define data modal
public class DemoItem
{
    public string Id;
    public string Name;
    public int ZipCode;
    public DateTime CreateTime;
}

// Impletement query method
public object QueryForCache(MySqlConnection sqlConnection, MySqlTransaction sqlTransaction, string key)
{
    DataRow row = Db.Query(DemoDB.TABLE_DEMO)
        .Select(
            DemoDB.COL_ID,
            DemoDB.COL_NAME,
            DemoDB.COL_ZIP_CODE,
            DemoDB.COL_CREATE_TIME
        )
        .Where(Db.EqColEqVal(DemoDB.COL_ID, key))
        .QueryFirst(sqlConnection, sqlTransaction);

    DemoItem item = new DemoItem();
    item.Id = row[DemoDB.COL_ID.Text].ToString();
    item.Name = row[DemoDB.COL_NAME.Text].ToString();
    item.ZipCode = Convert.ToInt32(row[DemoDB.COL_ZIP_CODE.Text]);
    item.CreateTime = (DateTime) DbDateTime.FromDb(row, DemoDB.COL_CREATE_TIME.Text);

    return item;
}
```

```csharp
// Use catche
DbCache catcheDemo = new DbCache(new MySqlConnection(/*db connection string*/));
DemoItem item123 = catcheDemo.DefaultCache(QueryForCache, "123") as DemoItem;			// from DB
DemoItem item123_again = catcheDemo.DefaultCache(QueryForCache, "123") as DemoItem;		// from Cache

// Remove catche
catcheDemo.RemoveCache("123");
```
