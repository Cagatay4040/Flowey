using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flowey.CORE.DataAccess.Abstract;
using Flowey.DOMAIN.Model.Abstract;

namespace Flowey.DOMAIN.Model.Concrete
{
    public class CommentAttachment : BaseEntity, IEntity
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public Guid CommentId { get; set; }

        public virtual Comment TaskComment { get; set; }
    }
}
