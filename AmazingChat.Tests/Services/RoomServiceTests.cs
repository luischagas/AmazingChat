using AmazingChat.Application.Common;
using AmazingChat.Application.Models;
using AmazingChat.Application.Services;
using AmazingChat.Domain.Entities;
using AmazingChat.Domain.Interfaces.Repositories;
using AmazingChat.Domain.Shared.Notifications;
using AmazingChat.Domain.Shared.Services;
using AmazingChat.Domain.Shared.UnitOfWork;
using AutoFixture;
using Moq;

namespace AmazingChat.Tests.Services;

public class RoomServiceTests
{
    private readonly RoomService _roomService;
    private readonly Mock<IRoomRepository> _mockRoomRepository;
    private readonly Fixture _fixture;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IChatHub> _mockChatHub;

    public RoomServiceTests()
    {
        _fixture = new Fixture();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockRoomRepository = new Mock<IRoomRepository>();
        _mockChatHub = new Mock<IChatHub>();
        var mockNotifier = new Mock<INotifier>();


        _roomService = new RoomService(_mockUnitOfWork.Object,
            mockNotifier.Object,
            _mockRoomRepository.Object,
            _mockChatHub.Object);
    }

    [Fact]
    public async Task GetAllRooms_Success()
    {
        var rooms = _fixture.CreateMany<Room>(5);

        _mockRoomRepository.Setup(m => m.GetAllAsync()).ReturnsAsync(rooms);

        var result = await _roomService.GetAll();

        var roomList = result as AppServiceResponse<List<RoomViewModel>>;

        Assert.Equal(rooms.Count(), roomList?.Data.Count);
    }

    [Fact]
    public async Task GetRoomById_Success()
    {
        var room = _fixture.Create<Room>();

        _mockRoomRepository.Setup(m => m.GetAsync(room.Id)).ReturnsAsync(room);

        var result = await _roomService.Get(room.Id);

        var roomObtained = result as AppServiceResponse<RoomViewModel>;

        Assert.Equal(room.Id, roomObtained?.Data.Id);
    }

    [Fact]
    public async Task CreateRoom_Success()
    {
        var room = _fixture.Create<Room>();

        _mockRoomRepository.Setup(m => m.GetByName(room.Name)).ReturnsAsync(() => null);

        _mockRoomRepository.Setup(m => m.AddAsync(room));

        _mockUnitOfWork.Setup(m => m.CommitAsync()).ReturnsAsync(() => true);

        _mockChatHub.Setup(m => m.CreateRoom(room.Id, room.Name));

        var roomViewModel = _fixture.Create<RoomViewModel>();

        var result = await _roomService.Create(roomViewModel);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task RemoveRoom_Success()
    {
        var room = _fixture.Create<Room>();

        _mockRoomRepository.Setup(m => m.GetAsync(room.Id)).ReturnsAsync(() => room);

        _mockRoomRepository.Setup(m => m.Delete(room));

        _mockUnitOfWork.Setup(m => m.CommitAsync()).ReturnsAsync(() => true);

        _mockChatHub.Setup(m => m.RemoveRoom(room.Id, room.Name));

        var result = await _roomService.Remove(room.Id);

        Assert.True(result.Success);
    }
}