using Oracle.ManagedDataAccess.Client;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace media
{
    public class DbTools
    {
        protected static string _conn = "";
        public static string GetConnectionString()
        {
            if (_conn == "")
            {
                var builder = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json");
                var config = builder.Build();
                _conn = config.GetConnectionString("DefaultConnection");
            }

            return _conn;
        }

        public static Image getImageFromDbByProdId(string prodId)
        {
            OracleConnection cn = null;
            Image b;

            try
            {
                cn = new OracleConnection(GetConnectionString());
                cn.Open();
                OracleCommand OracleCmd;
                OracleDataReader OracleRs;

                string query = @"WITH
                                INFO
                                AS
                                (
                                    SELECT
                                        dbms_lob.getlength(A.IMAGE) AS FILE_CONTENT_LENGTH, 
                                        MOD(dbms_lob.getlength(A.IMAGE),2000) AS MOD,
                                        CASE
                                            WHEN MOD(dbms_lob.getlength(A.IMAGE),2000) > 0 THEN TRUNC((dbms_lob.getlength(A.IMAGE)/2000) + 1)
                                            ELSE TRUNC(dbms_lob.getlength(A.IMAGE)/2000)
                                        END INTERATION_COUNT,
                                        a.IMAGE as FILE_CONTENT

                                    FROM PROD_IMAGE A WHERE A.PROD_ID = :PROD_ID AND  A.FIELD_INDEX = (select min(FIELD_INDEX) from PROD_IMAGE where PROD_ID = :PROD_ID)
                                )
                                ,OFFSETS AS
                                (
                                    SELECT
                                        (2000 * (ROWNUM-1)) + 1 AS OFFSET,
                                        I.MOD,
                                        I.FILE_CONTENT_LENGTH,
                                        I.INTERATION_COUNT,
                                        i.FILE_CONTENT
                                    FROM INFO I
                                    CONNECT BY LEVEL <= I.INTERATION_COUNT
                                )
                                ,RESULT AS
                                (
                                    SELECT
                                        DBMS_LOB.SUBSTR(O.FILE_CONTENT, 2000, O.OFFSET) AS CONTENT,
                                        O.OFFSET,
                                        O.MOD,
                                        O.FILE_CONTENT_LENGTH,

                                        O.INTERATION_COUNT
                                    FROM OFFSETS O
                                )
                                SELECT * FROM RESULT R ORDER BY R.OFFSET ASC";

                OracleCmd = new OracleCommand();
                OracleCmd.CommandText = query;
                OracleCmd.Connection = cn;
                OracleCmd.Parameters.Add("PROD_ID", OracleDbType.Varchar2).Value = prodId;
                OracleRs = OracleCmd.ExecuteReader();

                List<byte> list = new List<byte>();
           
                byte[] image = null;
                while (OracleRs.Read())
                {
                    if (OracleRs.GetValue(0) != DBNull.Value)
                    {
                        list.AddRange((byte[])OracleRs.GetValue(0));
                        
                    }
                }
                OracleRs.Close();

                image = list.ToArray();

                if (image != null)
                {
                    b = Image.Load(new MemoryStream(image));
                }
                else
                {

                    b = ImageTools.GetNoImage();
                }
            }
            catch (Exception ex)
            {
                b = ImageTools.GetNoImage();
            }
            finally
            {
                if (cn != null) cn.Close();
            }

            return b;
        }

        public static Image getImageFromDbByProdId_Test(string prodId)
        {
            OracleConnection cn = null;
            Image b = null;

            try
            {
                cn = new OracleConnection(GetConnectionString());
                cn.Open();
                OracleCommand OracleCmd = null;
                OracleDataReader OracleRs = null;

                string query = @"select IBLOB from MEMBERS where Id = :PROD_ID";

                OracleCmd = new OracleCommand();
                OracleCmd.CommandText = query;
                OracleCmd.Connection = cn;
                OracleCmd.Parameters.Add("PROD_ID", OracleDbType.Varchar2).Value = prodId;
                OracleRs = OracleCmd.ExecuteReader();

                byte[] image = null;
                while (OracleRs.Read())
                {
                    if (OracleRs.GetValue(0) != DBNull.Value)
                    {
                        image = (byte[])OracleRs.GetValue(0);
                    }
                    break;
                }
                OracleRs.Close();

                if (image != null)
                {
                    b = Image.Load(new MemoryStream(image));
                }
                else
                {
                    string _root = Directory.GetCurrentDirectory();
                  
                    b = Image.Load(_root + @"\\image\\Test.jpg");
                }
            }
            catch (Exception ex)
            {
                b = null;
            }
            finally
            {
                if (cn != null) cn.Close();
            }

            return b;
        }
    }
}
