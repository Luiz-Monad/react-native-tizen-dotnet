using ElmSharp;

namespace ReactNative.Views.Text
{
    public class ReactTextView : Label
    {
        public ReactTextView(EvasObject parent) : base(parent)
        {
        }

        public static string filterText(string strText)
        {
            strText.Replace("&", "&amp;");
            strText.Replace("<", "&lt;");
            strText.Replace(">", "&gt;");
            return strText;
        }
    }
}