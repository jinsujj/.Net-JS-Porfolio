using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CommonLibrary
{
    public class FileUtility
    {
        #region 중복된 파일명 뒤에 번호 붙이는 메서드 : GetFileNameWithNumbering
        /// <summary>
        /// GetFilePath : 파일명 뒤에 번호 붙이는 메서드
        /// </summary>
        /// <param name="dir">경로(c:\MyFiles)</param>
        /// <param name="name">Test.txt</param>
        /// <returns>Test(1).txt</returns>
        public static string GetFileNameWithNumbering(string dir, string name)
        {
            // 순수파일명 : Test
            string strName = Path.GetFileNameWithoutExtension(name);
            string strExt = Path.GetExtension(name);
            strName = strName.Substring(1);
            strExt = strExt.Substring(0, 4);

            bool blnExists = true;
            int i = 0;
            while (blnExists)
            {
                //Path.Combine(경로, 파일명) = 경로+파일명
                if (File.Exists(Path.Combine(dir, name)))
                {
                    name = strName + "(" + ++i + ")" + strExt; // Test(3).txt
                }
                else
                {
                    blnExists = false;
                    name = strName + strExt;
                }
            }
            return name;
        }
        #endregion
    }
}
