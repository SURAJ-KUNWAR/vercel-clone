using System.Text;

namespace API.Utils
{
    public class Utils
    {

        public static string Generate()
        {
            const string Subset  = "112435324398swiawdidejedbefefsdqjy";
            const int Length = 8;

            var random = new Random();
            var idBuilder = new StringBuilder();
            for(int i = 0; i < Length; i++)
            {
                int idx = random.Next(Subset.Length);
                idBuilder.Append(Subset[idx]);
            }
            return idBuilder.ToString();
        }

        public static List<string> GetAllFiles(string folderPath)
        {
            var response = new List<string>();
            var allFilesAndFolders = Directory.GetFileSystemEntries(folderPath);
            foreach(var file in allFilesAndFolders)
            {
                if (Directory.Exists(file))
                {
                    response.AddRange(GetAllFiles(file));
                }
                else
                {
                    response.Add(file);
                }
            }

            return response;

        }
    }
}
