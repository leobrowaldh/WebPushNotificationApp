using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Seeder;

public static class ProfilePictureSeeder
{
    public static readonly List<string> profilePictureUrls = new List<string>
    {
        "https://wallpapers.com/images/featured/cool-profile-picture-87h46gcobjl5e4xu.jpg",
        "https://images.pexels.com/photos/771742/pexels-photo-771742.jpeg?auto=compress&cs=tinysrgb&dpr=1&w=500",
        "https://wallpapers.com/images/hd/profile-picture-f67r1m9y562wdtin.jpg",
        "https://i.pinimg.com/564x/3c/1c/73/3c1c7364ed3445e25235b032ebc1dfe5.jpg",
        "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRMF7rNYRqdBhKmsTiW0pes2TrBJnzv7zqbjMp9W9J4cX4XK8jSeUmHBgHrgIt9AmANjxk&usqp=CAU",
        "https://wallpapers.com/images/featured/cute-profile-picture-s52z1uggme5sj92d.jpg",
        "https://i.pinimg.com/236x/85/59/09/855909df65727e5c7ba5e11a8c45849a.jpg",
        "https://img.freepik.com/premium-vector/hand-drawing-cartoon-girl-cute-girl-drawing-profile-picture_488586-692.jpg?w=360",
        "https://d22e6o9mp4t2lx.cloudfront.net/cms/pfp2_11cfcec183.webp",
        "https://static.vecteezy.com/system/resources/thumbnails/002/002/403/small/man-with-beard-avatar-character-isolated-icon-free-vector.jpg",
        "https://st3.depositphotos.com/15648834/17930/v/450/depositphotos_179308454-stock-illustration-unknown-person-silhouette-glasses-profile.jpg",
        "https://preview.redd.it/default-profile-picture-template-based-off-of-mf-doom-albums-v0-jdkp3xxvv4ua1.png?width=1000&format=png&auto=webp&s=877f3ba1d9f86651d50dceca01f223466d03f783",
        "https://cdn.vectorstock.com/i/1000v/79/38/little-girl-profile-avatar-isolated-cute-female-vector-21387938.jpg"
    };

    public static string RandomizeProfilePicture()
    {
        int randomIndex = Random.Shared.Next(profilePictureUrls.Count);
        return profilePictureUrls[randomIndex];
    }
}
