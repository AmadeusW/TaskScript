namespace TaskScript.Wpf
{
    public interface INotifyUser
    {
        void NotifyOfError(string path);

        void NotifyOfSuccess(string path);

        string Output { set; }
    }
}