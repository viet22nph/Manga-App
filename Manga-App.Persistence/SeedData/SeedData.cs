

using MangaApp.Domain.Entities.Identity;
using MangaApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MangaApp.Contract.Security.Resources;

namespace MangaApp.Persistence.SeedData;

public class SeedData
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    public SeedData(AppDbContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task SeedDataAsync()
    {
        await SeedGenreDataAsync();
        await SeedCountryDataAsync();
        await SeedUserAndRole();
        //await SeedRolesAndPermissions();
    }

    #region Thể loại truyện
    public async Task SeedGenreDataAsync()
    {
        if (await _context.Genres.AnyAsync()) { return; }
        var data = new List<Genre>
            {
                new Genre
               (
                   "Action",
                   "Thể loại này thường có nội dung về đánh nhau, bạo lực, hỗn loạn, với diễn biến nhanh"
                ),
                new Genre( "Adventure",
                  "Thể loại phiêu lưu, mạo hiểm, thường là hành trình của các nhân vật"),
                new Genre
                (
                   "Anime",
                  "Truyện đã được chuyển thể thành film Anime"
                ),
                new Genre
                (
                   "Chuyển Sinh",
                  "Thể loại này là những câu chuyện về người ở một thế giới này xuyên đến một thế giới khác, có thể là thế giới mang phong cách trung cổ với kiếm sĩ và ma thuật, hay thế giới trong game, hoặc có thể là bạn chết ở nơi này và được chuyển sinh đến nơi khác"
                ),
                new Genre
                (
                   "Cổ Đại",
                  "Truyện Cổ Đại Truyện có nội dung xảy ra ở thời cổ đại phong kiến."
                ),
                new Genre
                (
                   "Comedy",
                  "Thể loại có nội dung trong sáng và cảm động, thường có các tình tiết gây cười, các xung đột nhẹ nhàng"
                ),
                new Genre
                (
                   "Comic",
                  "Truyện tranh Châu Âu và Châu Mĩ"
                ),
                new Genre
                (
                   "Demons",
                   "Demons"
                ),
                new Genre
                (
                   "Detective",
                   "Thể loại điều tra, trinh thám."
                ),
                new Genre
                (
                   "Doujinshi",
                   "Thể loại truyện phóng tác do fan hay có thể cả những Mangaka khác với tác giả truyện gốc. Tác giả vẽ Doujinshi thường dựa trên những nhân vật gốc để viết ra những câu chuyện theo sở thích của mình"
                ),
                 new Genre
                (
                   "Drama",
                    "Thể loại mang đến cho người xem những cảm xúc khác nhau: buồn bã, căng thẳng thậm chí là bi phẫn"
                ),
                new Genre
                (
                   "Fantasy",
                    "Thể loại xuất phát từ trí tưởng tượng phong phú, từ pháp thuật đến thế giới trong mơ thậm chí là những câu chuyện thần tiên"
                ),
                new Genre
                (
                   "Gender Bender",
                    "Là một thể loại trong đó giới tính của nhân vật bị lẫn lộn: nam hoá thành nữ, nữ hóa thành nam..."

                ),
                new Genre
                (
                   "Harem",
                    "Thể loại truyện tình cảm, lãng mạn mà trong đó, nhiều nhân vật nữ thích một nam nhân vật chính"

                ),
                new Genre
                (
                   "Historical",
                    "Thể loại có liên quan đến thời xa xưa, lịch sử."

                ),
                new Genre
                (
                   "Horror",
                    "Horror là: rùng rợn, nghe cái tên là bạn đã hiểu thể loại này có nội dung thế nào. Nó làm cho bạn kinh hãi, khiếp sợ, ghê tởm, run rẩy, có thể gây sock - một thể loại không dành cho người yếu tim"

                ),
                new Genre
                (
                   "Huyền Huyễn",
                    "Truyện có yếu tố phép thuật, kỳ ảo… được đặt trong bối cảnh siêu tưởng (tiên giới, ma giới…)"

                ),
                new Genre
                (
                   "Isekai",
                    "Isekai, đôi khi còn được gọi là xuyên không hay chuyển sinh, là một tiểu thể loại light novel, manga, anime và video game kỳ ảo của Nhật Bản, xoay quanh một người bình thường được đưa đến hoặc bị mắc kẹt trong một vũ trụ song song."

                ),
                 new Genre
                (
                   "Josei",
                    "Thể loại của manga hay anime được sáng tác chủ yếu bởi phụ nữ cho những độc giả nữ từ 18 đến 30. Josei manga có thể miêu tả những lãng mạn thực tế , nhưng trái ngược với hầu hết các kiểu lãng mạn lí tưởng của Shoujo manga với cốt truyện rõ ràng, chín chắn"

                ),
                 new Genre
                (
                   "Mafia",
                    "Mafia"

                ),
                new Genre
                (
                   "Magic",
                    "Thể loại giả tưởng có tồn tại những sức mạnh siêu nhiên như thần chú, gây phép, vòng tròn ma thuật... "

                ),
                new Genre
                (
                   "Manhua",
                    "Truyện tranh của Trung Quốc"

                ),
                new Genre
                (
                   "Manhwa",
                    "Truyện tranh Hàn Quốc, đọc từ trái sang phải"

                ),
                new Genre
                (
                   "Martial Arts",
                    "Giống với tên gọi, bất cứ gì liên quan đến võ thuật trong truyện từ các trận đánh nhau, tự vệ đến các môn võ thuật như akido, karate, judo hay taekwondo, kendo, các cách né tránh"

                ),
                new Genre
                (
                   "Military",
                    "Truyện Quân Sự"

                ),
                new Genre
                (
                   "Mystery",
                    "Thể loại thường xuất hiện những điều bí ấn không thể lí giải được và sau đó là những nỗ lực của nhân vật chính nhằm tìm ra câu trả lời thỏa đáng"

                ),
                new Genre
                (
                   "Ngôn Tình",
                    "Truyện thuộc kiểu lãng mạn, kể về những sự kiện vui buồn trong tình yêu của nhân vật chính."

                ),
                new Genre
                (
                   "One shot",
                    "Những truyện ngắn, thường là 1 chapter"

                ),
                new Genre
                (
                   "Psychological",
                    "Thể loại liên quan đến những vấn đề về tâm lý của nhân vật ( tâm thần bất ổn, điên cuồng ...)"

                ),
                new Genre
                (
                   "Romance",
                    "Thường là những câu chuyện về tình yêu, tình cảm lãng mạn. Ớ đây chúng ta sẽ lấy ví dụ như tình yêu giữa một người con trai và con gái, bên cạnh đó đặc điểm thể loại này là kích thích trí tưởng tượng của bạn về tình yêu"

                ),
                new Genre
                (
                   "School Life",
                    "Trong thể loại này, ngữ cảnh diễn biến câu chuyện chủ yếu ở trường học"

                ),
                new Genre
                (
                   "Sci-fi",
                    "Bao gồm những chuyện khoa học viễn tưởng, đa phần chúng xoay quanh nhiều hiện tượng mà liên quan tới khoa học, công nghệ, tuy vậy thường thì những câu chuyện đó không gắn bó chặt chẽ với các thành tựu khoa học hiện thời, mà là do con người tưởng tượng ra"

                ),
                 new Genre
                (
                   "Seinen",
                    "Thể loại của manga thường nhằm vào những đối tượng nam 18 đến 30 tuổi, nhưng người xem có thể lớn tuổi hơn, với một vài bộ truyện nhắm đến các doanh nhân nam quá 40. Thể loại này có nhiều phong cách riêng biệt , nhưng thể loại này có những nét riêng biệt, thường được phân vào những phong cách nghệ thuật rộng hơn và phong phú hơn về chủ đề, có các loại từ mới mẻ tiên tiến đến khiêu dâm"

                ),
                 new Genre
                (
                   "Shoujo",
                    "Đối tượng hướng tới của thể loại này là phái nữ. Nội dung của những bộ manga này thường liên quan đến tình cảm lãng mạn, chú trọng đầu tư cho nhân vật (tính cách,...)"

                ),
                 new Genre
                (
                   "Shoujo Ai",
                    "Thể loại quan hệ hoặc liên quan tới đồng tính nữ, thể hiện trong các mối quan hệ trên mức bình thường giữa các nhân vật nữ trong các manga, anime"

                ),
                 new Genre
                (
                   "Shounen",
                    "Đối tượng hướng tới của thể loại này là phái nam. Nội dung của những bộ manga này thường liên quan đến đánh nhau và/hoặc bạo lực (ở mức bình thường, không thái quá)"

                ),
                 new Genre
                (
                   "Shounen Ai",
                    "Thể loại có nội dung về tình yêu giữa những chàng trai trẻ, mang tính chất lãng mạn nhưng ko đề cập đến quan hệ tình dục"

                ),
                  new Genre
                (
                   "Slice of life",
                    "Nói về cuộc sống đời thường"

                ),
                   new Genre
                (
                   "Sports",
                    "Đúng như tên gọi, những môn thể thao như bóng đá, bóng chày, bóng chuyền, đua xe, cầu lông,... là một phần của thể loại này"

                ),
                    new Genre
                (
                   "Supernatural",
                    "Thể hiện những sức mạnh đáng kinh ngạc và không thể giải thích được, chúng thường đi kèm với những sự kiện trái ngược hoặc thách thức với những định luật vật lý"

                ),
                new Genre
                (
                   "Tragedy",
                    "Thể loại chứa đựng những sự kiện mà dẫn đến kết cục là những mất mát hay sự rủi ro to lớn"

                ),
                new Genre
                (
                   "Trọng Sinh",
                    "Trọng sinh là thể loại mà nhân vật chính vì một lý do nào đó chết đi rồi đầu thai vào kiếp khác nhưng vẫn giữ lại được kí ức của mình ở kiếp trước."

                ),
                new Genre
                (
                   "Truyện Màu",
                    "Tổng hợp truyện tranh màu, rõ, đẹp"

                ),
                new Genre
                (
                   "Webtoon",
                    "Là truyện tranh được đăng dài kỳ trên internet của Hàn Quốc chứ không xuất bản theo cách thông thường"

                ),
                new Genre
                (
                   "Xuyên Không",
                    "Xuyên Không, Xuyên Việt là thể loại nhân vật chính vì một lý do nào đó mà bị đưa đến sinh sống ở một không gian hay một khoảng thời gian khác. Nhân vật chính có thể trực tiếp xuyên qua bằng thân xác mình hoặc sống lại bằng thân xác người khác."

                ),
            };

        await _context.Genres.AddRangeAsync(data);
        _context.SaveChanges();
    }
    #endregion Thể loại truyện

    #region Quốc gia
    public async Task SeedCountryDataAsync()
    {
        if (await _context.Countries.AnyAsync()) return;
        var countries = new List<Country> {
                new Country
                (
                   "vn",
                   "Việt Nam"
                ),
                new Country
                (
                   "cn",
                   "Trung Quốc"
                ),
                new Country
                ("us",
                   "Mỹ"

                ),
                new Country
                (
                    "jp",
                   "Nhật Bản"

                ),
                new Country
                (
                   "kr",
                   "Hàn Quốc"
                )
            };

        await _context.Countries.AddRangeAsync(countries);
        _context.SaveChanges();
    }
    #endregion Quốc gia


    #region User and role
    public async Task SeedUserAndRole()
    {

            // Seed Resources
        if( !await _context.Resources.AnyAsync())
        {
            var resources = new List<Resource>
            {
                new Resource
                {
                    Name = Resources.Manga,
                    Description = "Manga manager permission",
                    Permissions = new List<Permission>
                    {
                        new Permission
                        {
                            Name = Contract.Security.Permissions.Permission.Manga.All,
                            Description ="Tất cả quyền manga"
                        },
                        new Permission
                        {
                            Name = Contract.Security.Permissions.Permission.Manga.Create,
                            Description ="Quyền tạo manga"
                        },
                        new Permission
                        {
                            Name = Contract.Security.Permissions.Permission.Manga.Update,
                            Description ="Quyển cập nhật manga"
                        },
                        new Permission
                        {
                            Name = Contract.Security.Permissions.Permission.Manga.Delete,
                            Description ="Quyển xóa manga"
                        },
                        new Permission
                        {
                            Name = Contract.Security.Permissions.Permission.Manga.Approve,
                            Description ="Quyển duyệt manga"
                        }
                    }
                },
                new Resource
                {
                    Name = Resources.Chapter,
                    Description = "Chapter manager permission",
                    Permissions = new List<Permission>
                    {
                        new Permission
                        {
                            Name = Contract.Security.Permissions.Permission.Chapter.All,
                            Description ="Tất cả quyền chapter"
                        },
                        new Permission
                        {
                            Name = Contract.Security.Permissions.Permission.Chapter.Create,
                            Description ="Quyền tạo chapter"
                        },
                        new Permission
                        {
                            Name = Contract.Security.Permissions.Permission.Chapter.Update,
                            Description ="Quyển cập nhật chapter"
                        },
                        new Permission
                        {
                            Name = Contract.Security.Permissions.Permission.Chapter.Delete,
                            Description ="Quyển xóa chapter"
                        },
                        new Permission
                        {
                            Name = Contract.Security.Permissions.Permission.Chapter.Approve,
                            Description ="Quyển duyệt chapter"
                        }
                    }
                },
                new Resource
                {
                    Name = Resources.Chapter,
                    Description = "User manager permission",
                    Permissions = new List<Permission>
                    {
                        new Permission
                        {
                            Name = Contract.Security.Permissions.Permission.User.All,
                            Description ="Tất cả quyền người dùng"
                        },
                        new Permission
                        {
                            Name = Contract.Security.Permissions.Permission.User.Create,
                            Description ="Quyền tạo người dùng"
                        },
                        new Permission
                        {
                            Name = Contract.Security.Permissions.Permission.User.Update,
                            Description ="Quyển cập nhật người dùng"
                        },
                        new Permission
                        {
                            Name = Contract.Security.Permissions.Permission.User.Ban,
                            Description ="Quyển xóa cấm người dùng"
                        },
                        new Permission
                        {
                            Name = Contract.Security.Permissions.Permission.User.Unban,
                            Description ="Quyền gỡ cấm người dùng"
                        },
                        new Permission
                        {
                             Name = Contract.Security.Permissions.Permission.User.AssignRole,
                            Description ="Quyền quản lý role cho người dùng"
                        }
                    }
                }
            };

            await _context.Resources.AddRangeAsync(resources);
            _context.SaveChanges();
        }    
            

            // Step 3: Tạo các role Admin và User
            var adminRole = await _roleManager.FindByNameAsync("Admin");
            if (adminRole == null)
            {
                adminRole = new AppRole { Name = "Admin", CreatedDate = DateTimeOffset.UtcNow };
                await _roleManager.CreateAsync(adminRole);
            }

            var userRole = await _roleManager.FindByNameAsync("User");
            if (userRole == null)
            {
                userRole = new AppRole { Name = "User", CreatedDate = DateTimeOffset.UtcNow };
                await _roleManager.CreateAsync(userRole);
            }
            var adminUser = AppUser.CreateNewUser("admin", "viet22nph@gmail.com");
            var user = await _userManager.FindByEmailAsync(adminUser.Email);

            if (user == null)
            {
                var result = await _userManager.CreateAsync(adminUser, "123456");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
                
         await _context.SaveChangesAsync();

        var permissons = await _context.Permissions.ToListAsync();
        // add role admin permission
        foreach (var permission in permissons)
        {
            var exists = _context.RolePermissions.Any(rp => rp.RoleId == adminRole.Id && rp.PermissionId == permission.Id);
            if (!exists)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = permission.Id
                });
            }
        }
        await _context.SaveChangesAsync();
    }
    #endregion
}