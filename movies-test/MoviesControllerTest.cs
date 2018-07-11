using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using movies_api.Controllers;
using movies_api.Models;
using movies_api.Repositories;

namespace movies_test
{
    [TestClass]
    public class MoviesControllerTest
    {
        private List<Movie> _TestMovies;

        public MoviesControllerTest()
        {
            _TestMovies = GetTestMovies();
        }

        #region " GetMovies "
        [TestMethod]
        public async Task GetMovies_ReturnsOkNegotiatedContentResult_GivenInvalidModel()
        {
            // Arrange
            var mockRepo = new Mock<IMovieRepository>();
            var controller = new MoviesController(mockRepo.Object);
            controller.ModelState.AddModelError("Error", "Test Error");

            // Act
            var result = await controller.GetMovies();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<List<Movie>>));
        }

        [TestMethod]
        public async Task GetMovies_ReturnsOkNegotiatedContentResult_GivenEmptyModel()
        {
            // Arrange
            var mockRepo = new Mock<IMovieRepository>();
            mockRepo.Setup(repo => repo.List()).Returns(Task.FromResult(new List<Movie>()));
            var controller = new MoviesController(mockRepo.Object);

            // Act
            var result = await controller.GetMovies();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<List<Movie>>));
        }

        [TestMethod]
        public async Task GetAllMovies_ShouldReturnAllMovies()
        {
            // Arrange
            var mockRepo = new Mock<IMovieRepository>();
            mockRepo.Setup(repo => repo.List()).Returns(Task.FromResult(_TestMovies));
            var controller = new MoviesController(mockRepo.Object);

            // Act
            var result = await controller.GetMovies();

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<List<Movie>>));

            var movies = ((OkNegotiatedContentResult<List<Movie>>)result).Content;
            Assert.AreEqual(movies.Count(), _TestMovies.Count());

            var movie = movies.FirstOrDefault();
            Assert.AreEqual(movie.Title, _TestMovies[0].Title);
        }
        #endregion

        #region " GetMovie "
        [TestMethod]
        public async Task GetMovie_ReturnsNotFoundResult_GivenInvalidModel()
        {
            // Arrange
            int testId = 1;
            var mockRepo = new Mock<IMovieRepository>();
            var controller = new MoviesController(mockRepo.Object);
            controller.ModelState.AddModelError("Error", "Test Error");

            // Act
            var result = await controller.GetMovie(testId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetMovie_ReturnsNotFound_GivenEmptyModel()
        {
            // Arrange
            int testId = 1;
            var mockRepo = new Mock<IMovieRepository>();
            mockRepo.Setup(repo => repo.GetById(testId)).Returns(Task.FromResult((new List<Movie>()).FirstOrDefault(m => m.ID == testId)));
            var controller = new MoviesController(mockRepo.Object);

            // Act
            var result = await controller.GetMovie(testId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetMovie_ReturnsNotFound_ForInvalidId()
        {
            // Arrange
            int testId = 10;
            var mockRepo = new Mock<IMovieRepository>();
            mockRepo.Setup(repo => repo.GetById(testId)).Returns(Task.FromResult(_TestMovies.FirstOrDefault(m => m.ID == testId)));
            var controller = new MoviesController(mockRepo.Object);

            // Act
            var result = await controller.GetMovie(testId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetMovie_ReturnsOkObject_WithMovie()
        {
            // Arrange
            int testId = 1;
            var mockRepo = new Mock<IMovieRepository>();
            mockRepo.Setup(repo => repo.GetById(testId))
                .Returns(Task.FromResult(_TestMovies.FirstOrDefault(m => m.ID == testId)));
            var controller = new MoviesController(mockRepo.Object);

            // Act
            var result = await controller.GetMovie(testId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<Movie>));

            var movie = ((OkNegotiatedContentResult<Movie>)result).Content;
            Assert.AreEqual(movie.Title, _TestMovies[0].Title);
        }
        #endregion

        #region " PutMovie "
        [TestMethod]
        public async Task PutMovie_ReturnsInvalidModelStateResult_GivenInvalidModel()
        {
            // Arrange & Act
            int testId = 1;
            var mockRepo = new Mock<IMovieRepository>();
            var controller = new MoviesController(mockRepo.Object);
            controller.ModelState.AddModelError("Error", "Test Error");

            // Act
            var result = await controller.PutMovie(testId, new Movie());

            // Assert
            Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public async Task PutMovie_ReturnsBadRequest_ForInvalidId()
        {
            // Arrange
            int testId = 10;
            var mockRepo = new Mock<IMovieRepository>();
            mockRepo.Setup(repo => repo.GetById(testId))
                 .Returns(Task.FromResult(_TestMovies.FirstOrDefault(m => m.ID == testId)));
            var controller = new MoviesController(mockRepo.Object);

            // Act
            var result = await controller.PutMovie(testId, new Movie());

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task PutMovie_ReturnsStatusCodeResult()
        {
            // Arrange
            int testId = 1;
            string testTitle = "Updated Movie Title 1";
            var testMovie = _TestMovies.FirstOrDefault(m => m.ID == testId);
            var mockRepo = new Mock<IMovieRepository>();
            mockRepo.Setup(repo => repo.GetById(testId))
                .Returns(Task.FromResult(testMovie));
            var controller = new MoviesController(mockRepo.Object);

            testMovie.Title = testTitle;
            mockRepo.Setup(repo => repo.Update(testMovie))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await controller.PutMovie(testId, testMovie);

            // Assert
            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));

            mockRepo.Verify();
        }
        #endregion

        #region " PostMovie "
        [TestMethod]
        public async Task PostMovie_ReturnsInvalidModelStateResult_GivenInvalidModel()
        {
            // Arrange & Act
            var mockRepo = new Mock<IMovieRepository>();
            var controller = new MoviesController(mockRepo.Object);
            controller.ModelState.AddModelError("Error", "Test Error");

            // Act
            var result = await controller.PostMovie(new Movie());

            // Assert
            Assert.IsInstanceOfType(result, typeof(InvalidModelStateResult));
        }

        [TestMethod]
        public async Task PostMovie_ReturnsCreatedAtRouteNegotiatedContentResult()
        {
            // Arrange
            var testMovie = new Movie()
            {
                ID = 4,
                Title = "Test Movie 4",
                ReleaseDate = new DateTime(2004, 04, 04),
                Genre = "Test Genre 4",
                Price = 1,
                Rating = "Test Rating 4"
            };
            var mockRepo = new Mock<IMovieRepository>();
            var controller = new MoviesController(mockRepo.Object);

            // Act
            var result = await controller.PostMovie(testMovie);

            // Assert
            Assert.IsInstanceOfType(result, typeof(CreatedAtRouteNegotiatedContentResult<Movie>));

            var movie = ((CreatedAtRouteNegotiatedContentResult<Movie>)result).Content;
            Assert.AreEqual(movie.Title, testMovie.Title);
        }
        #endregion

        #region " DeleteMovie "
        [TestMethod]
        public async Task DeleteMovie_ReturnsNotFoundResult_GivenInvalidModel()
        {
            // Arrange & Act
            int testId = 1;
            var mockRepo = new Mock<IMovieRepository>();
            var controller = new MoviesController(mockRepo.Object);
            controller.ModelState.AddModelError("Error", "Test Error");

            // Act
            var result = await controller.DeleteMovie(testId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeleteMovie_ReturnsNotFound_ForInvalidId()
        {
            // Arrange
            int testId = 10;
            var mockRepo = new Mock<IMovieRepository>();
            mockRepo.Setup(repo => repo.GetById(testId))
                 .Returns(Task.FromResult(_TestMovies.FirstOrDefault(m => m.ID == testId)));
            var controller = new MoviesController(mockRepo.Object);

            // Act
            var result = await controller.DeleteMovie(testId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeleteMovie_ReturnsOkObject()
        {
            // Arrange
            int testId = 1;
            var testMovie = _TestMovies.FirstOrDefault(m => m.ID == testId);
            var mockRepo = new Mock<IMovieRepository>();
            mockRepo.Setup(repo => repo.GetById(testId))
                .Returns(Task.FromResult(testMovie));
            var controller = new MoviesController(mockRepo.Object);

            mockRepo.Setup(repo => repo.Delete(testMovie))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await controller.DeleteMovie(testId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkNegotiatedContentResult<Movie>));

            mockRepo.Verify();
        }
        #endregion

        private List<Movie> GetTestMovies()
        {
            var testMovies = new List<Movie>()
            {
                new Movie()
                {
                    ID = 1,
                    Title = "Test Movie 1",
                    ReleaseDate = new DateTime(1901, 1, 1),
                    Genre = "Test Genre 1",
                    Price = 1,
                    Rating = "Test Rating 1"
                },
                new Movie()
                {
                    ID = 2,
                    Title = "Test Movie 2",
                    ReleaseDate = new DateTime(1902, 2, 2),
                    Genre = "Test Genre 2",
                    Price = 2,
                    Rating = "Test Rating 2"
                },
                new Movie()
                {
                    ID = 3,
                    Title = "Test Movie 3",
                    ReleaseDate = new DateTime(1903, 3, 3),
                    Genre = "Test Genre 3",
                    Price = 3,
                    Rating = "Test Rating 3"
                }
            };

            return testMovies;
        }
    }
}
