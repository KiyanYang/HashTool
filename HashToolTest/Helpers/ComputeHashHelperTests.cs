using System.Collections.Generic;

using HashTool.Helpers.Hashs;
using HashTool.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HashTool.Helpers.Tests
{
    [TestClass()]
    public class ComputeHashHelperTests
    {
        [TestMethod()]
        public void HashStringTest()
        {
            Dictionary<string, Dictionary<string, string>> textInput = new()
            {
                ["123456789"] = new()
                {
                    [Hash.CRC32.Name] = "CBF43926",
                    [Hash.MD5.Name] = "25F9E794323B453885F5181F1B624D0B",
                    [Hash.SHA1.Name] = "F7C3BC1D808E04732ADF679965CCC34CA7AE3441",
                    [Hash.SHA256.Name] = "15E2B0D3C33891EBB0F1EF609EC419420C20E320CE94C65FBC8C3312448EB225",
                    [Hash.SHA384.Name] = "EB455D56D2C1A69DE64E832011F3393D45F3FA31D6842F21AF92D2FE469C499DA5E3179847334A18479C8D1DEDEA1BE3",
                    [Hash.SHA512.Name] = "D9E6762DD1C8EAF6D61B3C6192FC408D4D6D5F1176D0C29169BC24E71C3F274AD27FCD5811B313D681F7E55EC02D73D499C95455B6B5BB503ACF574FBA8FFE85",
                    [Hash.QuickXor.Name] = "MZDBDGhQAxvcAAc5CQAAAAAAAAA=",
                },
                ["Kiyan Yang"] = new()
                {
                    [Hash.CRC32.Name] = "CF497CF7",
                    [Hash.MD5.Name] = "844CA63144A1F17E370A1370019378AE",
                    [Hash.SHA1.Name] = "BCF3626FCD0890B9FC5035A766021BDF0D9DEC4F",
                    [Hash.SHA256.Name] = "707F6AE7FE9573FD6CC69D5DB98FC9127B6486BDFA9E9108DA00E36CF60D3399",
                    [Hash.SHA384.Name] = "5D36218D431F9D59B406772CF4B0A417ABB1179BB6FC660FA21B3BD75D8C99D75253502B6B51BA8DF31F251959163899",
                    [Hash.SHA512.Name] = "533D711AA9BC05705DAEE1C7FDF01D23991A6068C7F3B5BEC4FD4B42B6C57CD5162DBEC2370E401B24AAA61DC828631F4F26B227EF140A888A69AE49786A43AA",
                    [Hash.QuickXor.Name] = "S0hDHsLgBhBkIQxuMgMAAAAAAAA=",
                },
            };

            HashInputModel hashInput = new()
            {
                Mode = "文本"
            };
            hashInput.CheckBoxItems.Add(new() { Content = Hash.CRC32.Name, IsChecked = true });
            hashInput.CheckBoxItems.Add(new() { Content = Hash.MD5.Name, IsChecked = true });
            hashInput.CheckBoxItems.Add(new() { Content = Hash.SHA1.Name, IsChecked = true });
            hashInput.CheckBoxItems.Add(new() { Content = Hash.SHA256.Name, IsChecked = true });
            hashInput.CheckBoxItems.Add(new() { Content = Hash.SHA384.Name, IsChecked = true });
            hashInput.CheckBoxItems.Add(new() { Content = Hash.SHA512.Name, IsChecked = true });
            hashInput.CheckBoxItems.Add(new() { Content = Hash.QuickXor.Name, IsChecked = true });

            foreach (var kvp in textInput)
            {
                hashInput.Input = kvp.Key;
                var result = ComputeHashHelper.HashString(hashInput);
                Assert.AreEqual(kvp.Value.Count, result.Items.Count);
                foreach (var j in result.Items)
                {
                    Assert.AreEqual(kvp.Value[j.Name], j.Value);
                }
            }
        }
    }
}
