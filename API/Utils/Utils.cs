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
    }
}
