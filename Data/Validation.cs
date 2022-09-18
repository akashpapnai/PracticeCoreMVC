namespace PracticeCoreMVC.Data
{
    public class Validation
    {
        public bool containsWhiteSpace(string str)
        {
            foreach(var i in str)
            {
                if(i == ' ')
                {
                    return true;
                }
            }
            return false;
        }
        public bool containsNumeralOrSpecialCharacters(string str)
        {
            foreach (var i in str)
            {
                if (!((i-'A' >=0 && i-'A' < 26) || (i-'a' >=0 && i-'a' < 26)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
