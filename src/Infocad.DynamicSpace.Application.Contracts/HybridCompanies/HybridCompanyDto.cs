using System;
using Volo.Abp.Application.Dtos;

namespace Infocad.DynamicSpace.HybridCompanies
{
    public class HybridCompanyDto : ExtensibleEntityDto<Guid>
    {
        public Guid? DynamicEntityId
        {
            get => _dynamicEntityId; set
            {
                if (value == Guid.Empty)
                    _dynamicEntityId = null;
                else
                    _dynamicEntityId = value;
            }
        }
        private Guid? _dynamicEntityId { get; set; }

        public string RagioneSociale { get; set; }
        public string Cognome { get; set; }
        public string Indirizzo { get; set; }
        public string Telefono { get; set; }
    }
}
