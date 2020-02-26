using System.Threading.Tasks;
using WePrint.Common.Slicer.Interface;
using WePrint.Common.Slicer.Models;

namespace WePrint.Slicer.Interface
{
    public interface ISlicer
    {
        SliceReport Slice(string fileName, string filePath);
    }
}
