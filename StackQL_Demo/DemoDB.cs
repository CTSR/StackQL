using StackQL;

namespace StackQL_Demo
{
    public class DemoDB
    {
        static public readonly string TABLE_DEMO = "DEMO";
        static public readonly DbColumn COL_ID = Db.Col("Id");
        static public readonly DbColumn COL_NAME = Db.Col("Name");
        static public readonly DbColumn COL_ZIP_CODE = Db.Col("ZipCode");
        static public readonly DbColumn COL_CREATE_TIME = Db.Col("CreateTime");
    }
}
