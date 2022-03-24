using Base_service.DatabaseManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTest : BaseDatabaseCommands
    {
        [TestMethod]
        public void Test0_Connection_Test()
        {
            Assert.IsNotNull(BaseConnection);
        }

        [TestMethod]
        public void Test1_Select()
        {
            var result = BaseSelect(
                "users",
                "`users`.`id`,`username`,`password`,`locations`.`name` AS 'location',`permission`,`active`",
                new string[,] { { "`users`.`id`", "=", $"''" } },
                "INNER JOIN `locations` ON `users`.`locationId` = `locations`.`id` INNER JOIN `regions` ON `locations`.`regionId` = `regions`.`id`"
                );

            Assert.IsNotNull(result.Item1.Rows);
            Assert.AreEqual("", result.Item2);
        }

        [TestMethod]
        public void Test2_Insert()
        {
            var result = BaseInsert("users", "`username`, `password`, `locationId`, `permission`, `active`", "'username','password','1','1','1'");

            Assert.AreEqual(1, result.Item1);
        }

        [TestMethod]
        public void Test3_Update()
        {
            var result = BaseUpdate("users", new string[,] { {"`permission`", "'2'" } }, $"`username`='username'");

            Assert.AreEqual(1, result.Item1);
        }

        [TestMethod]
        public void Test4_Delete()
        {
            var result = BaseDelete("users", $"`username`='username'");

            Assert.AreEqual(1, result.Item1);
        }
    }
}
