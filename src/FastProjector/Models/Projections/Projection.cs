using System;
using FastProjector.Contracts;
using FastProjector.Models.TypeInformations;
using SourceCreationHelper;
using SourceCreationHelper.Interfaces;

namespace FastProjector.Models.Projections
{
    internal abstract class Projection : IProjection
    {
        private readonly CollectionTypeInformation _destinationTypeInformation;

        protected Projection(CollectionTypeInformation destinationTypeInformation)
        {
            _destinationTypeInformation = destinationTypeInformation;
        }

        public abstract ISourceText CreateProjection(IModelMapService mapService, ISourceText parameterName);

        protected static ICallSourceText CreateSelectExpression(string paramName, ISourceText returnExpression)
        {
            var selectExpression = SourceCreator.CreateLambdaExpression()
                .AddParameter(paramName)
                .AssignBodyExpression(returnExpression);

            return SourceCreator.CreateCall("Select")
                .AddArgument(selectExpression);
        }

        protected PropertyCastResult CreateIEnumerableCasting(IModelMapService mapService)
        {
            var iEnumerableTypeOfProjected =
                CreateIEnumerableTypeInformation(_destinationTypeInformation.GetCollectionType());

            var enumerableCastInfo = mapService.CastType(iEnumerableTypeOfProjected,
                _destinationTypeInformation);

            return enumerableCastInfo;
        }

        private static TypeInformation CreateIEnumerableTypeInformation(TypeInformation genericType)
        {
            return new GenericCollectionTypeInformation(CollectionTypeEnum.System_Collections_Generic_IEnumerable_T,
                genericType);
        }

        public static Projection Create(CollectionTypeInformation sourceTypeInformation,
            CollectionTypeInformation destinationTypeInformation)
        {
            if (!sourceTypeInformation.GetCollectionType()
                    .HasSameCategory(destinationTypeInformation.GetCollectionType()))
            {
                return null;
            }

            var collectionType = sourceTypeInformation.GetCollectionType();
            
            return collectionType switch
            {
                ClassTypeInformation => new ClassProjection(sourceTypeInformation, destinationTypeInformation),
                CollectionTypeInformation => NestedProjection.Create(sourceTypeInformation, destinationTypeInformation),
                GenericClassTypeInformation => new ClassProjection(sourceTypeInformation, destinationTypeInformation),
                PrimitiveTypeInformation => new PrimitiveProjection(sourceTypeInformation, destinationTypeInformation),
                _ => null
            };
        }
    }
}