using System.Collections.Generic;
using System.Linq;
using System.Net;
using NUnit.Framework;
using SoftAPIClient.Example.Models.GitHub;
using SoftAPIClient.Example.Services.GitHub;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Example.Tests.GitHub
{
    public class GitHubRepositoryProjectE2ETests : AbstractTest
    {
        private const string ProjectNameToCreate = "TestProjectApi";
        private const string ProjectDescriptionToCreate = ProjectNameToCreate + "Description";
        private const string ProjectDescriptionToCreateUpdated = ProjectDescriptionToCreate + "Updated";

        private const string ColumnNameToCreate = "TestColumnApi";
        private const string ColumnNameToCreateUpdated = ColumnNameToCreate + "Updated";

        private int _projectId; 
        private int _columnId; 

        [Test]
        [Ignore("Ignored due to update in GH API")]
        public void VerifyE2EFlowWithRepositoryProjectManagement()
        {
            //Create Project
            var responseCreatedProject = GetService<IGitHubRepositoryProjectService>()
                .CreateRepositoryProject(new GitHubBodyRequest
                {
                    Name = ProjectNameToCreate,
                    Body = ProjectDescriptionToCreate
                })
                .Invoke();
            Assert.AreEqual(HttpStatusCode.Created, responseCreatedProject.HttpStatusCode);

            var createdProjectBody = responseCreatedProject.Body;
            Assert.AreEqual(ProjectNameToCreate, createdProjectBody.Name);
            Assert.AreEqual(ProjectDescriptionToCreate, createdProjectBody.Body);
            _projectId = createdProjectBody.Id;

            //Get Project by Id
            var responseGetProjectById = GetService<IGitHubProjectService>()
                .GetProjectById(_projectId).Invoke();
            Assert.AreEqual(HttpStatusCode.OK, responseGetProjectById.HttpStatusCode);

            var getProjectByIdBody = responseGetProjectById.Body;
            Assert.AreEqual(ProjectNameToCreate, getProjectByIdBody.Name);
            Assert.AreEqual(ProjectDescriptionToCreate, getProjectByIdBody.Body);

            //Update Project
            var responseUpdatedProject = GetService<IGitHubProjectService>()
                .UpdateProjectById(_projectId, new GitHubBodyRequestShort
                {
                    Name = ProjectNameToCreate,
                    Body = ProjectDescriptionToCreateUpdated
                })
                .Invoke();
            Assert.AreEqual(HttpStatusCode.OK, responseUpdatedProject.HttpStatusCode);

            var updatedProjectBody = responseUpdatedProject.Body;
            Assert.AreEqual(_projectId, updatedProjectBody.Id);
            Assert.AreEqual(ProjectNameToCreate, updatedProjectBody.Name);
            Assert.AreEqual(ProjectDescriptionToCreateUpdated, updatedProjectBody.Body);

            //Create Column
            var createdColumnResponse = GetService<IGitHubColumnService>()
                .CreateProjectColumn(_projectId, new GitHubBodyRequestShort
                {
                    Name = ColumnNameToCreate
                }).Invoke();

            Assert.AreEqual(HttpStatusCode.Created, createdColumnResponse.HttpStatusCode);

            var createdColumnBody = createdColumnResponse.Body;
            Assert.AreEqual(ColumnNameToCreate, createdColumnBody.Name);
            _columnId = createdColumnBody.Id;

            //Get Column by Id
            var responseGetColumnById = GetService<IGitHubColumnService>()
                .GetColumnById(_columnId).Invoke();
            Assert.AreEqual(HttpStatusCode.OK, responseGetColumnById.HttpStatusCode);

            var getColumnByIdBody = responseGetColumnById.Body;
            Assert.AreEqual(ColumnNameToCreate, getColumnByIdBody.Name);

            //Update Column
            var responseUpdatedColumn = GetService<IGitHubColumnService>()
                .UpdateColumnById(_columnId, new GitHubBodyRequestShort
                {
                    Name = ColumnNameToCreateUpdated
                })
                .Invoke();
            Assert.AreEqual(HttpStatusCode.OK, responseUpdatedColumn.HttpStatusCode);

            var updatedColumnBody = responseUpdatedColumn.Body;
            Assert.AreEqual(_columnId, updatedColumnBody.Id);
            Assert.AreEqual(ColumnNameToCreateUpdated, updatedColumnBody.Name);

            //Move Column
            var responseMovedColumn = GetService<IGitHubColumnService>()
                .MoveColumnById(_columnId, new GitHubBodyRequest
                {
                    Position = "last"
                })
                .Invoke();
            Assert.AreEqual(HttpStatusCode.Created, responseMovedColumn.HttpStatusCode);

            //Delete Column
            var responseDeletedColumn = DeleteColumnById(_columnId);
            Assert.AreEqual(HttpStatusCode.NoContent, responseDeletedColumn.HttpStatusCode);

            //Delete Project
            var responseDeletedProject = DeleteProjectById(_projectId);
            Assert.AreEqual(HttpStatusCode.NoContent, responseDeletedProject.HttpStatusCode);
        }

        [TearDown]
        public void TearDown()
        {
            DeleteColumnById(_columnId);
            if (_columnId == 0)
            {
                GetColumns().ForEach(p => DeleteColumnById(p.Id));
            }
            DeleteProjectById(_projectId);
            if (_projectId == 0)
            {
                GetProjects().ForEach(p => DeleteProjectById(p.Id));
            }
        }

        private List<GitHubResponse> GetProjects()
        {
           return GetService<IGitHubRepositoryProjectService>().GetProjects().Invoke().Body?
               .Where(b => ProjectNameToCreate.Equals(b?.Name)).ToList();
        }

        private List<GitHubResponse> GetColumns()
        {
            return GetService<IGitHubRepositoryProjectService>().GetProjects().Invoke().Body?
                .Where(b => ColumnNameToCreateUpdated.Equals(b?.Name) || ColumnNameToCreate.Equals(b?.Name)).ToList();
        }

        private ResponseGeneric<GitHubErrorResponse> DeleteProjectById(int id)
        {
            return GetService<IGitHubProjectService>().DeleteProjectById(id).Invoke();
        }

        private ResponseGeneric<GitHubErrorResponse> DeleteColumnById(int id)
        {
            return GetService<IGitHubColumnService>()
                .DeleteColumnById(id)
                .Invoke();
        }

    }
}
