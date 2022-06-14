using FastProjector.Contracts;
using FastProjector.Models.TypeInformations;
using SourceCreationHelper;
using SourceCreationHelper.Interfaces;

namespace FastProjector.Models.Projections
{
    internal sealed class PrimitiveNestedProjection : NestedProjection
    {
        public PrimitiveNestedProjection(Projection innerProjection,
            CollectionTypeInformation destinationTypeInformation) : base(innerProjection, destinationTypeInformation)
        { }
    }
}