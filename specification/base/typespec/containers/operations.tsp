import "@typespec/http";
import "@typespec/openapi";

import "./models.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

@route("/containers")
@tag("Containers")
interface Containers {
  @get
  @operationId("ListContainers")
  @list
  listContainers(
    /**
     * A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
     * default is 20.
     */
    @query limit?: int32 = 20,

    /**
     * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
     * for descending order.
     */
    @query order?: ContainerCollectionOrder,

    /**
     * A cursor for use in pagination. `after` is an object ID that defines your place in the list.
     * For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
     * subsequent call can include after=obj_foo in order to fetch the next page of the list.
     */
    @query @continuationToken after?: string,
  ): ContainerListResource | ContainersErrorResponse;

  @post
  @operationId("CreateContainer")
  createContainer(
    @body
    body: CreateContainerBody,
  ): ContainerResource | ContainersErrorResponse;

  @get
  @route("{container_id}")
  @operationId("RetrieveContainer")
  retrieveContainer(
    @path
    container_id: string,
  ): ContainerResource | ContainersErrorResponse;

  @delete
  @route("{container_id}")
  @operationId("DeleteContainer")
  deleteContainer(
    @path
    container_id: string,
  ): {
    id: string;
    object: "container.deleted";
    deleted: true;
  } | ContainersErrorResponse;

  @post
  @route("{container_id}/files")
  @operationId("CreateContainerFile")
  createContainerFile(
    @path
    container_id: string,

    @header
    contentType: "multipart/form-data",

    @multipartBody
    body: CreateContainerFileBody,
  ): ContainerFileResource | ContainersErrorResponse;

  @get
  @route("{container_id}/files")
  @operationId("ListContainerFiles")
  @list
  listContainerFiles(
    @path
    container_id: string,

    /**
     * A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
     * default is 20.
     */
    @query limit?: int32 = 20,

    /**
     * Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
     * for descending order.
     */
    @query order?: ContainerFileCollectionOrder,

    /**
     * A cursor for use in pagination. `after` is an object ID that defines your place in the list.
     * For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
     * subsequent call can include after=obj_foo in order to fetch the next page of the list.
     */
    @query @continuationToken after?: string,
  ): ContainerFileListResource | ContainersErrorResponse;

  @get
  @route("{container_id}/files/{file_id}")
  @operationId("RetrieveContainerFile")
  retrieveContainerFile(
    @path
    container_id: string,

    @path
    file_id: string,
  ): ContainerFileResource | ContainersErrorResponse;

  @delete
  @route("{container_id}/files/{file_id}")
  @operationId("DeleteContainerFile")
  deleteContainerFile(
    @path
    container_id: string,

    @path
    file_id: string,
  ): {
    id: string;
    object: "container.file.deleted";
    deleted: true;
  } | ContainersErrorResponse;

  @get
  @route("{container_id}/files/{file_id}/content")
  @operationId("RetrieveContainerFileContent")
  retrieveContainerFileContent(
    @path
    container_id: string,

    @path
    file_id: string,
  ): {
    @encode("base64") @body file_content: bytes;
  } | ContainersErrorResponse;
}
