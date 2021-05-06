// ***********************************************************************
// Author           : John Thiriet
//                    https://github.com/johnthiriet/AsyncVoid
// ***********************************************************************

using System.Threading.Tasks;
using System.Windows.Input;

namespace FolderObserver.Common
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }
}
