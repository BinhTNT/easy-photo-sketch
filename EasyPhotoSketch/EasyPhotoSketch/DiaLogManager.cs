namespace EasyPhotoSketch
{
    public class DialogManager
    {
        public delegate void ShowDialogCallback(string title, string message);
        public delegate void DialogActionCallback_OK();
        public delegate void DialogActionCallback_Cancel();
        private ShowDialogCallback m_showDialogCallback = null;
        private DialogActionCallback_OK m_dialogActionCallback_OK = null;
        private DialogActionCallback_Cancel m_dialogActionCallback_Cancel = null;
        private static DialogManager instance = null;

        public enum DIALOG_ACTION
        {
            DA_OK,
            DA_CANCEL,
            NONE
        }

        public static DialogManager Instance()
        {
            if(instance == null)
            {
                instance = new DialogManager();
            }
            return instance;
        }

        public void SetShowDiaLogCallback(ShowDialogCallback showDialogCallback)
        {
            m_showDialogCallback = showDialogCallback;
        }
        public void SetDialogActionCallback_OK(DialogActionCallback_OK dialogCallbackOk)
        {
            m_dialogActionCallback_OK = dialogCallbackOk;
        }
        public void SetDialogActionCallback_Cancel(DialogActionCallback_Cancel dialogCallbackCancel)
        {
            m_dialogActionCallback_Cancel = dialogCallbackCancel;
        }

        public void ShowDialog(string title, string message)
        {
            m_showDialogCallback?.Invoke(title, message);
        }

        public void DoAction(DIALOG_ACTION dialogAction)
        {
            switch(dialogAction)
            {
                case DIALOG_ACTION.DA_OK:
                    m_dialogActionCallback_OK?.Invoke();
                    break;
                case DIALOG_ACTION.DA_CANCEL:
                    m_dialogActionCallback_Cancel?.Invoke();
                    break;
                default:
                    break;
            }

        }
    }
}