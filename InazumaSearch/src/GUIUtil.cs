using System.Windows.Forms;

namespace InazumaSearch
{
    public class GUIUtil
    {
        #region ダイアログボックス表示

        public static void ShowInformationMessage(string message, string title = "情報")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static void ShowInformationMessage(IWin32Window owner, string message, string title = "情報")
        {
            MessageBox.Show(owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public static void ShowErrorMessage(IWin32Window owner, string message)
        {
            MessageBox.Show(owner, message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static bool Confirm(string message, bool defaultNo = false)
        {
            var res = MessageBox.Show(
                  message
                , "確認"
                , MessageBoxButtons.YesNo
                , MessageBoxIcon.Warning
                , (defaultNo ? MessageBoxDefaultButton.Button2 : MessageBoxDefaultButton.Button1));

            return (res == DialogResult.Yes);
        }

        public static bool Confirm(IWin32Window owner, string message, bool defaultNo = false)
        {
            var res = MessageBox.Show(owner
                , message
                , "確認"
                , MessageBoxButtons.YesNo
                , MessageBoxIcon.Warning
                , (defaultNo ? MessageBoxDefaultButton.Button2 : MessageBoxDefaultButton.Button1));

            return (res == DialogResult.Yes);
        }

        #endregion
    }
}
