using ChatDashboard.Api.DTOs;
using ChatDashboard.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace ChatDashboard.Api.Services
{
    
    public class MessagesService
    {
        private readonly AppDbContext _context;

        public MessagesService(AppDbContext context)
        {
            _context = context;
        }

        /*
        public async Task<List<MessageWithTargetDto>> GetMessagesReceivedByUserAsync(string userId)
        {
            var messages = await (from m in _context.Messages
                                    join mt in _context.MessageTargets
                                    on m.MessageId equals mt.MessageId
                                    where mt.TargetId == userId
                                    select new MessageWithTargetDto
                                    {
                                        MessageId = m.MessageId,
                                        SenderId = m.SenderId,
                                        MessageBody = m.MessageBody,
                                        CreatedAt = m.CreatedAt,
                                        TargetId = mt.TargetId
                                    })
                                    .ToListAsync();
            
            return messages;
        }

        public async Task<List<MessageWithTargetDto>> GetMessagesBySenderAsync(string senderId)
        {
            return await (from m in _context.Messages
                            join mt in _context.MessageTargets
                            on m.MessageId equals mt.MessageId
                            where m.SenderId == senderId
                            select new MessageWithTargetDto
                            {
                                MessageId = m.MessageId,
                                SenderId = m.SenderId,
                                MessageBody = m.MessageBody,
                                CreatedAt = m.CreatedAt,
                                TargetId = mt.TargetId
                            })
                            .OrderByDescending(x => x.CreatedAt)
                            .ToListAsync();
        }*/
    }

    public class MessageEncryptionService
    {
        public readonly RSA _rsa;

        public MessageEncryptionService()
        {
            _rsa = RSA.Create();
            var privateKeyBase64 = "MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQC0n+ZGf+2MyZmVHmLftLvURHDdqWfdd58HICCuAjGRl5Sn6x0H+lxuHfKUK//zJbOluSboETAPyS8DH7DQfQ17k/UMeDeLa/CdqstAYEkZVmyxtBURvQLTRVznvpHfX9x1zo6Y+w4FJap7bmAsUf5FVCcT3IJFrbzqFN2qmv47nxcjo77B6RXH5bf6IyvAIxy/vLdNbI3V+E/E3JObFV+xfm2VH0x568i2L1uP94owaLAl9u7kL074O5xby1GEHea0nbSGftUc+CU7gZxzeWMrY6JHyIrxRsA9dbJMk39uLOwh/HNOMezWNHeAxN1hsy1IbNj6S835AlEj3wQ/St4PAgMBAAECggEAAp2aWNlIaCMc30uIZgNhgR91392wqAqYpYtE4Mpjy9OI+oSe9cbAl3N0BoTS9hSp6jCu44GGB+DauWJbPHMg9v2qG7yeemMo56BT3FmRU/iO7E1/QSgYl5rn8mBzbGuJL0GMz+g6cRC7EfbJoXk4IQG0jkEet9OBHsuT7C+XxP4Nr64oggdPd0HxmVgRqyUUBTMDznvBiyujSBMkVAnSJW4tJPkNm4G9EXos9ojbgj0XKjEQYge4wsqYXG2zBP0dZ6FqgT0KdhL7Do6v3u371Rqi930eHuPhpHVvN5KRlHI86Owc0QUSzFJ4vBeCGxv/AdIO+alqzbT1AtDyvK1KkQKBgQDkOqCzXum4csMmENkYvOLn0gn6DzeQBFV3NDgB9ho4x41I3d7tTh1e3xKBflasD5xBW9cHUv8SGELm+BEmm59wyLeMPMUr1fJrWc2c+t1BlvGQq+ruuJCgNly/tvrsWoiBzvjoZ01kLA1ngWqicDALK+g+IyTAZvIuW9btu5G/PwKBgQDKmmLv3xR99A0jsSY+btZAKI3q4R2tAEovu7l875MEfYpKCQRCIgOxOkaz8P0Yr5O9JdmEkeh7EnC6dZNNNo+khWhYpBbziVvDi6AkW6CGBhovvMa52sQhexqOMDrSVxD3EDJp66rerhn6DrYfghFhQbAy/Zh9ccIbzf0/PJz9MQKBgCqkfaQmBrtMzgONwFJr8GVqDC0prLL+7E0Sd6h+KBYVyuMjeWSTZM92FIoXZfBOFl7r3vhiXIAwAkgPb3zsNNUo24qbTCfNmLbQF6fOA4sevdHPHV0vJO6aWagEc0u3+qtuVXlu27nstTBysM7a8U8VDBaYEGTWl0dsJ3NRyYCXAoGAX92BZwLlTVvHQ9Y0xGHpmt4VmMgAJLX4ZHGtKQKrS/qjDrkJfMlfeeKbRdWHzMB3ZY6dR+9KU8G7+V5c3rIscap2X0r1WTLrarGMiueYIxKMHX5zgcmT+EupmxITIyfzbRQK5reOWGOFAx8m6e2/j4dnehkoqGLFeCaV5/AkUGECgYBIbEKRrt+wAe1bO4rQWb5/Epg8hiDq9DMy/eA3LNFVZVxGCDzJBnvHIilrZVE2pPCs1RWyJKtg0hfJjdLrpooRTtGcfMw1oGc2+dSVrykZCnC3jy3AwLJRjBC3BSS3lGKVt2ZFlDaSA5ZlmdPzHW75NHbmIF1vbtkdlncdPfkjcA==";

            var privateKeyBytes = Convert.FromBase64String(privateKeyBase64);
            _rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);

            var publicKeyBytes = _rsa.ExportSubjectPublicKeyInfo();
            var publicKeyBase64 = Convert.ToBase64String(publicKeyBytes);
        }

        public string GetPublicKey()
        {
            var publicKeyBytes = _rsa.ExportSubjectPublicKeyInfo();
            return Convert.ToBase64String(publicKeyBytes);
        }

        //Encrypt message
        public (string encryptedMessage, string encryptedAesKey, string hash) EncryptMessage(string plainMessage)
        {
            // Generate AES key
            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.GenerateKey();
            aes.GenerateIV();

             // Encrypt message with AES
            var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(plainMessage);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            var encryptedMessage = Convert.ToBase64String(encryptedBytes);

            // Encrypt AES key with RSA
            var aesKeyWithIv = aes.Key.Concat(aes.IV).ToArray();
            var encryptedAesKey = _rsa.Encrypt(aesKeyWithIv, RSAEncryptionPadding.OaepSHA256);
            var encryptedKeyString = Convert.ToBase64String(encryptedAesKey);

            // Compute HMAC using AES key
            using var hmac = new HMACSHA256(aes.Key);
            var hashBytes = hmac.ComputeHash(encryptedBytes);
            var hash = Convert.ToBase64String(hashBytes);

            return (encryptedMessage, encryptedKeyString, hash);
        }

        // Decrypt message
        public string DecryptMessage(string encryptedMessage, string encryptedKey, string hashMessage)
        {
            try{
                // Decrypt AES key using RSA private key
                var encryptedKeyBytes = Convert.FromBase64String(encryptedKey);
                var aesKeyWithIv = _rsa.Decrypt(encryptedKeyBytes, RSAEncryptionPadding.OaepSHA256);

                var aesKey = aesKeyWithIv.Take(32).ToArray(); // 256-bit key
                var aesIV = aesKeyWithIv.Skip(32).ToArray();   // 128-bit IV

                // Decrypt message with AES
                using var aes = Aes.Create();
                aes.Key = aesKey;
                aes.IV = aesIV;

                var decryptor = aes.CreateDecryptor();
                var encryptedBytes = Convert.FromBase64String(encryptedMessage);
                var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

                var decryptedText = Encoding.UTF8.GetString(decryptedBytes);

                // Verify HMAC
                using var hmac = new HMACSHA256(aesKey);
                var computedHash = Convert.ToBase64String(
                    hmac.ComputeHash(encryptedBytes)
                );

                if (computedHash != hashMessage) {
                    Console.WriteLine($"Message hash mismatch: {encryptedMessage}");
                    return null; // Skip corrupted message
                }

                return decryptedText;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to decrypt message: {ex.Message}");
                return null; // Skip corrupted message
            }
        }

        public string ComputeHash(string plainMessage)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainMessage));
            return Convert.ToBase64String(hashBytes);
        }
    }
}
