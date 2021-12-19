using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Notification
{
    public interface INotifier<T>
    {
        void Notify(T data);
    }
}
