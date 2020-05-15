using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PollApp
{
    interface IDocumentStorage<T> where T : Document
    {
        Task CreateDocument(T document);

        Task<T> GetDocument(DocumentId documentId);
    }
}
