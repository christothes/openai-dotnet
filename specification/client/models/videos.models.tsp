import "../../base/typespec/videos/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;
using TypeSpec.Http;

namespace OpenAI;

union VideoCollectionOrder {
   string,
   Ascending: "asc",
   Descending: "desc",
}

@access(Access.public)
@usage(Usage.input)
model VideoCollectionOptions {
   /**
    * A cursor for use in pagination. `after` is an object ID that defines your place in the list.
    * For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
    * subsequent call can include after=obj_foo in order to fetch the next page of the list.
    */
   @query @continuationToken afterId?: string;

   /**
    * A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
    * default is 20.
    */
   @query pageSizeLimit?: int32 = 20;

   /**
    * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
    * for descending order.
    */
   @query order?: VideoCollectionOrder;
}
