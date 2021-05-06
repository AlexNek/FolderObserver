using System;

namespace FolderObserver.Common
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex, string text);
    }
}
