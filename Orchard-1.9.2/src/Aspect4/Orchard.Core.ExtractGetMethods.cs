using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspectDotNet;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Common.Models;
using Orchard.Core.Containers.Models;
using Orchard.Core.Containers.Services;

namespace Aspect4 {
    public class O : Aspect
    {
        public static IContentDefinitionManager _contentDefinitionManager;
        public static IContentManager _contentManager;

        //конструктор не внедряется
        //[AspectAction("%after %call *ContainerService.ctor(IContentDefinitionManager, IContentManager, IRandomizer) && args(..)")]
        public static void  GetFields(IContentDefinitionManager contentDefinitionManager, IContentManager contentManager, IRandomizer randomizer)
        {
            _contentDefinitionManager = contentDefinitionManager;
            _contentManager = contentManager;
        }

        //метод внедряется
        [AspectAction("%instead %call *ContainerService.GetContainableTypes()")]
        public static IEnumerable<ContentTypeDefinition> GetContainableTypes() {
            return _contentDefinitionManager.ListTypeDefinitions().Where(td => td.Parts.Any(p => p.PartDefinition.Name == typeof (ContainablePart).Name)).OrderBy(x => x.DisplayName);
        }

        //метод не внедряется
       // [AspectAction("%instead %call *ContainerService.GetContainerTypes()")]
        public static IEnumerable<ContentTypeDefinition> GetContainerTypes()
        {
            return _contentDefinitionManager.ListTypeDefinitions().Where(td => td.Parts.Any(p => p.PartDefinition.Name == typeof (ContainerPart).Name)).OrderBy(x => x.DisplayName);
        }

      //метод не внедряется
        //[AspectAction("%after %call *ContainerService.GetContainers(VersionOptions) && args(..)")]
        public static IEnumerable<ContainerPart> GetContainers(VersionOptions options = null) {
            return GetContainersQuery(options).List();
        }

     //метод не внедряется
      // [AspectAction("%instead %call *ContainerService.GetContainersQuery(VersionOptions) && args(..)")]
        public static IContentQuery<ContainerPart> GetContainersQuery(VersionOptions options = null) {
            options = options ?? VersionOptions.Published;
            return _contentManager.Query<ContainerPart>(options);
        }

     //метод внедряется
    //  [AspectAction("%instead %call *ContainerService.Get(int, VersionOptions) && args(..)")]
        public static ContainerPart Get(int id, VersionOptions options = null) {
            options = options ?? VersionOptions.Published;
            return _contentManager.Get<ContainerPart>(id, options);
        }       
    }
}
