using Application.Domain;
using Application.Exceptions;
using Application.Services;
using Application.Interfaces;
using Moq;

namespace TestLayer;

[TestClass]
public class TagTests
{
    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void GetById_IdZeroOrNegative_ThrowsArgumentException(int tagId) {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TagService>>();
        var tagRepoMock = new Mock<ITagRepository>();
        tagRepoMock.Setup(r => r.FindById(tagId))
            .Throws(new ArgumentException($"Tag ID must be greater than zero: {tagId}"));
        var tagService = new TagService(tagRepoMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => tagService.GetById(tagId));
        Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
        Assert.AreEqual($"Error retrieving tag with id: {tagId}", ex.Message);
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(12)]
    public void GetById_TagNotFoundException(int tagId) {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TagService>>();
        var tagRepoMock = new Mock<ITagRepository>();
        tagRepoMock.Setup(r => r.FindById(tagId)).Returns((Tag)null!);
        var tagService = new TagService(tagRepoMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<TagNotFoundException>(() => tagService.GetById(tagId));
        Assert.AreEqual($"No tag with id: {tagId} found.", ex.Message);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void GetById_ReturnsTag(int tagId) {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TagService>>();
        var tagRepoMock = new Mock<ITagRepository>();
        var tag = new Tag(tagId, "Test Tag", 1);
        tagRepoMock.Setup(r => r.FindById(tagId)).Returns(tag);
        var tagService = new TagService(tagRepoMock.Object, loggerMock.Object);

        var result = tagService.GetById(tagId);

        Assert.IsNotNull(result);
        Assert.AreEqual(tagId, result.Id);
        Assert.AreEqual("Test Tag", result.Name);
    }

    [TestMethod]
    public void GetById_DatabaseException_IsWrapped() {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TagService>>();
        var tagRepoMock = new Mock<ITagRepository>();
        tagRepoMock.Setup(r => r.FindById(It.IsAny<int>())).Throws(new DatabaseException("DB error"));
        var tagService = new TagService(tagRepoMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => tagService.GetById(1));
        Assert.AreEqual("Database error retrieving tag with id: 1", ex.Message);
        Assert.IsInstanceOfType(ex.InnerException, typeof(DatabaseException));
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void GetByUserId_IdZeroOrNegative_ThrowsArgumentException(int userId) {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TagService>>();
        var tagRepoMock = new Mock<ITagRepository>();
        tagRepoMock.Setup(r => r.FindByUserId(userId))
            .Throws(new ArgumentException($"User ID must be greater than zero: {userId}"));
        var tagService = new TagService(tagRepoMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => tagService.GetByUserId(userId));
        Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
        Assert.AreEqual($"Error retrieving tags for user_id: {userId}", ex.Message);
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(12)]
    public void GetByUserId_NoTagsFound_ThrowsException(int userId) {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TagService>>();
        var tagRepoMock = new Mock<ITagRepository>();
        tagRepoMock.Setup(r => r.FindByUserId(userId)).Throws(new KeyNotFoundException());
        var tagService = new TagService(tagRepoMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => tagService.GetByUserId(userId));
        Assert.AreEqual($"No tags found for user_id: {userId}", ex.Message);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void GetByUserId_ReturnsTagList(int userId) {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TagService>>();
        var tagRepoMock = new Mock<ITagRepository>();
        var tags = new List<Tag> { new Tag(1, "Test Tag", userId) };
        tagRepoMock.Setup(r => r.FindByUserId(userId)).Returns(tags);
        var tagService = new TagService(tagRepoMock.Object, loggerMock.Object);

        var result = tagService.GetByUserId(userId);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Test Tag", result[0].Name);
    }

    [TestMethod]
    public void GetByUserId_DatabaseException_IsWrapped() {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TagService>>();
        var tagRepoMock = new Mock<ITagRepository>();
        tagRepoMock.Setup(r => r.FindByUserId(It.IsAny<int>())).Throws(new DatabaseException("DB error"));
        var tagService = new TagService(tagRepoMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => tagService.GetByUserId(1));
        Assert.AreEqual("Database error retrieving tags for user_id: 1", ex.Message);
        Assert.IsInstanceOfType(ex.InnerException, typeof(DatabaseException));
    }

    [TestMethod]
    [DataRow("Test Tag", -1)]
    [DataRow("Test Tag", 0)]
    public void Add_UserIdZeroOrNegative_ThrowsInvalidDataException(string name, int userId) {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TagService>>();
        var tagRepoMock = new Mock<ITagRepository>();
        tagRepoMock.Setup(r => r.Add(It.IsAny<Tag>()))
            .Throws(new ArgumentException("User ID must be greater than zero."));
        var tagService = new TagService(tagRepoMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<InvalidDataException>(() => tagService.Add(name, userId));
        Assert.AreEqual("Invalid tag data: User ID must be greater than zero.", ex.Message);
    }

    [TestMethod]
    [DataRow("", 1)]
    [DataRow("   ", 1)]
    public void Add_NameIsEmpty_ThrowsInvalidDataException(string name, int userId) {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TagService>>();
        var tagRepoMock = new Mock<ITagRepository>();
        tagRepoMock.Setup(r => r.Add(It.IsAny<Tag>()))
            .Throws(new ArgumentException("Name cannot be empty."));
        var tagService = new TagService(tagRepoMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<InvalidDataException>(() => tagService.Add(name, userId));
        Assert.AreEqual("Invalid tag data: Name cannot be empty.", ex.Message);
    }

    [TestMethod]
    public void Add_DatabaseException_IsWrapped() {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TagService>>();
        var tagRepoMock = new Mock<ITagRepository>();
        tagRepoMock.Setup(r => r.Add(It.IsAny<Tag>())).Throws(new DatabaseException("DB error"));
        var tagService = new TagService(tagRepoMock.Object, loggerMock.Object);

        var ex = Assert.ThrowsException<Exception>(() => tagService.Add("Test", 1));
        Assert.AreEqual("Database error while adding a tag.", ex.Message);
        Assert.IsInstanceOfType(ex.InnerException, typeof(DatabaseException));
    }

    [TestMethod]
    public void Add_ValidTag_ReturnsTag() {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TagService>>();
        var tagRepoMock = new Mock<ITagRepository>();
        var tag = new Tag(1, "Test Tag", 1);
        tagRepoMock.Setup(r => r.Add(It.IsAny<Tag>())).Returns(tag);
        var tagService = new TagService(tagRepoMock.Object, loggerMock.Object);

        var result = tagService.Add("Test Tag", 1);

        Assert.IsNotNull(result);
        Assert.AreEqual("Test Tag", result.Name);
        Assert.AreEqual(1, result.UserId);
    }

    [TestMethod]
    public void Edit_TagNotFound_ReturnsFalse() {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TagService>>();
        var tagRepoMock = new Mock<ITagRepository>();
        tagRepoMock.Setup(r => r.Edit(It.IsAny<Tag>())).Throws(new TagNotFoundException("Not found"));
        var tagService = new TagService(tagRepoMock.Object, loggerMock.Object);

        var result = tagService.Edit(new Tag(1, "Test", 1));
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Edit_DatabaseException_ReturnsFalse() {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TagService>>();
        var tagRepoMock = new Mock<ITagRepository>();
        tagRepoMock.Setup(r => r.Edit(It.IsAny<Tag>())).Throws(new DatabaseException("DB error"));
        var tagService = new TagService(tagRepoMock.Object, loggerMock.Object);

        var result = tagService.Edit(new Tag(1, "Test", 1));
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Edit_ValidTag_ReturnsTrue() {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TagService>>();
        var tagRepoMock = new Mock<ITagRepository>();
        tagRepoMock.Setup(r => r.Edit(It.IsAny<Tag>())).Returns(true);
        var tagService = new TagService(tagRepoMock.Object, loggerMock.Object);

        var result = tagService.Edit(new Tag(1, "Test", 1));
        Assert.IsTrue(result);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void Delete_IdZeroOrNegative_ReturnsFalse(int tagId) {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TagService>>();
        var tagRepoMock = new Mock<ITagRepository>();
        tagRepoMock.Setup(r => r.FindById(tagId)).Throws(new ArgumentException("Invalid ID"));
        var tagService = new TagService(tagRepoMock.Object, loggerMock.Object);

        var result = tagService.Delete(tagId);
        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow(4)]
    [DataRow(12)]
    public void Delete_TagNotFound_ReturnsFalse(int tagId) {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TagService>>();
        var tagRepoMock = new Mock<ITagRepository>();
        tagRepoMock.Setup(r => r.FindById(tagId)).Throws(new TagNotFoundException("Not found"));
        var tagService = new TagService(tagRepoMock.Object, loggerMock.Object);

        var result = tagService.Delete(tagId);
        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    public void Delete_ValidTag_ReturnsTrue(int tagId) {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TagService>>();
        var tagRepoMock = new Mock<ITagRepository>();
        var tag = new Tag(tagId, "Test", 1);
        tagRepoMock.Setup(r => r.FindById(tagId)).Returns(tag);
        tagRepoMock.Setup(r => r.Delete(tag)).Returns(true);
        var tagService = new TagService(tagRepoMock.Object, loggerMock.Object);

        var result = tagService.Delete(tagId);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Delete_DatabaseException_ReturnsFalse() {
        var loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<TagService>>();
        var tagRepoMock = new Mock<ITagRepository>();
        var tag = new Tag(1, "Test", 1);
        tagRepoMock.Setup(r => r.FindById(1)).Returns(tag);
        tagRepoMock.Setup(r => r.Delete(It.IsAny<Tag>())).Throws(new DatabaseException("DB error"));
        var tagService = new TagService(tagRepoMock.Object, loggerMock.Object);

        var result = tagService.Delete(1);
        Assert.IsFalse(result);
    }
}