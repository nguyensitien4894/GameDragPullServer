using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Hosting;

namespace TraditionGame.Utilities.Chat
{
    public static class ChatFilter
    {
        private static readonly object lockLoadBadWords = new object();
        private static readonly object lockLoadBadLinks = new object();
        private static readonly object lockLoadBanUsers = new object();

        private const string RegexAcceptChars = @"[^aáàảãạăắằẳẵặâấầẩẫậđeéèẻẽẹêếềểễệiíìỉĩịoóòỏõọôốồổỗộơớờởỡợuúùủũụưứừửữựyýỳỷỹỵAÁÀẢÃẠĂẮẰẲẴẶÂẤẦẨẪẬĐEÉÈẺẼẸÊẾỀỂỄỆIÍÌỈĨỊOÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢUÚÙỦŨỤƯỨỪỬỮỰYÝỲỶỸỴ\w\s\d]+";
        private static readonly List<string> BadWords = new List<string>();
        private static readonly List<string> BadLinks = new List<string>();
        private static readonly List<string> BanUsers = new List<string>();
        private static readonly ConcurrentDictionary<string, string> ReplaceVNs = new ConcurrentDictionary<string, string>();

        private static readonly string BANUSERS_FILE = HostingEnvironment.MapPath("~/App_Data/Chat/Data/banusers.txt");
        private static readonly string BADWORDS_FILE = HostingEnvironment.MapPath("~/App_Data/Chat/Data/badwords.txt");
        private static readonly string BADLINKS_FILE = HostingEnvironment.MapPath("~/App_Data/Chat/Data/badlinks.txt");

        static ChatFilter()
        {
            if (BadWords == null || BadWords.Count < 1)
            {
                if (Monitor.TryEnter(lockLoadBadWords, 60000))
                {
                    try
                    {
                        NLogManager.LogMessage(string.Format("Load file bad word: {0}", BADWORDS_FILE));
                        if (File.Exists(BADWORDS_FILE))
                        {
                            string[] allText = File.ReadAllLines(BADWORDS_FILE);
                            BadWords = new List<string>(allText);
                        }
                    }
                    catch (Exception e)
                    {
                        NLogManager.PublishException(e);
                    }
                    finally
                    {
                        Monitor.Exit(lockLoadBadWords);
                    }
                }
            }

            if (BadLinks == null || BadLinks.Count < 1)
            {
                if (Monitor.TryEnter(lockLoadBadLinks, 60000))
                {
                    try
                    {
                        NLogManager.LogMessage(string.Format("Load file bad links: {0}", BADLINKS_FILE));
                        if (File.Exists(BADLINKS_FILE))
                        {
                            string[] allText = File.ReadAllLines(BADLINKS_FILE);
                            BadLinks = new List<string>(allText);
                        }
                    }
                    catch (Exception e)
                    {
                        NLogManager.PublishException(e);
                    }
                    finally
                    {
                        Monitor.Exit(lockLoadBadLinks);
                    }
                }
            }

            if (BanUsers == null || BanUsers.Count < 1)
            {
                if (Monitor.TryEnter(lockLoadBanUsers, 60000))
                {
                    try
                    {
                        NLogManager.LogMessage(string.Format("Load file banned user: {0}", BANUSERS_FILE));
                        if (File.Exists(BANUSERS_FILE))
                        {
                            string[] allText = File.ReadAllLines(BANUSERS_FILE);
                            BanUsers.AddRange(allText);
                        }
                    }
                    catch (Exception e)
                    {
                        NLogManager.PublishException(e);
                    }
                    finally
                    {
                        Monitor.Exit(lockLoadBanUsers);
                    }
                }
            }

        }

        public static string CutOff(string input, string pattern = " ")
        {
            for (int i = 0; i < pattern.Length; i++)
            {
                input = input.Replace(pattern[i].ToString(), "");
            }

            return input;
        }

        public static string ReplaceVN(string input)
        {
            if (ReplaceVNs == null || ReplaceVNs.Count < 1)
            {
                if (Monitor.TryEnter(ReplaceVNs, 60000))
                {
                    try
                    {
                        ReplaceVNs.TryAdd("[@ÅÄäẢÃÁÀẠảãáàạÂĂẨẪẤẦẬẩẫấầậẲẴẮẰẶẳẵắằặ]+", "a");
                        ReplaceVNs.TryAdd("[ß]+", "b");
                        ReplaceVNs.TryAdd("[Ç€]+", "c");
                        ReplaceVNs.TryAdd("[ËẺẼÉÈẸẻẽéèẹÊỂỄẾỀỆêểễếềệ]+", "e");
                        ReplaceVNs.TryAdd("[ÏιỈĨÍÌỊỉĩíìị]+", "i");
                        ReplaceVNs.TryAdd("[ØÖöΘ☻❂ỎÕÓÒỌỏõóòọÔỔỖỐỒỘôổỗốồộƠỞỠỚỜỢơởỡớờợ0]+", "o");
                        ReplaceVNs.TryAdd("[Šš]+", "s");
                        ReplaceVNs.TryAdd("[τ]+", "t");
                        ReplaceVNs.TryAdd("[ÜỦŨÙỤÚủũúùụỬỮỨỪỰửữứừự]+", "u");
                        ReplaceVNs.TryAdd("[•,;:]+", ".");
                    }
                    finally
                    {
                        Monitor.Exit(ReplaceVNs);
                    }
                }
            }

            foreach (string key in ReplaceVNs.Keys)
            {
                try
                {
                    Regex regx = new Regex(key, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    input = regx.Replace(input, ReplaceVNs[key]);
                    //NLogManager.LogMessage(key + " : " + input);
                } catch (Exception ex) {
                    NLogManager.PublishException(ex);
                }
            }

            return input;
        }

        public static string RemoveUnAcceptChars(string input)
        {
            Regex regx = new Regex(RegexAcceptChars, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            string output = regx.Replace(input, "*");
            return output;
        }

        public static string RemoveBadWords(string input, out bool Flag )
        {
            Flag = false;
            int bwLength = BadWords.Count;
            for (int i = 0; i < bwLength; i++)
            {
                try
                {
                    string bw = BadWords[i];
                    if (bw.StartsWith("regex::", StringComparison.OrdinalIgnoreCase))
                    {
                        bw = bw.Substring(7);
                        Regex regx = new Regex(bw, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        if (regx.IsMatch(input))
                            Flag = true;
                        input = regx.Replace(input, "***");
                    }
                    else
                    {
                        int countLength = input.Length;
                        input = input.Replace(bw, "***", StringComparison.OrdinalIgnoreCase);
                        if (input.Length != countLength)
                            Flag = true;
                    }
                }
                catch (Exception ex)
                {
                    NLogManager.PublishException(ex);
                }
            }

            return input;
        }

        public static string RemoveBadWords(string input)
        {
            int bwLength = BadWords.Count;
            for (int i = 0; i < bwLength; i++)
            {
                try
                {
                    string bw = BadWords[i];
                    if (bw.StartsWith("regex::", StringComparison.OrdinalIgnoreCase))
                    {
                        bw = bw.Substring(7);
                        Regex regx = new Regex(bw, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        input = regx.Replace(input, "***");
                    }
                    else
                    {
                        input = input.Replace(bw, "***", StringComparison.OrdinalIgnoreCase);
                    }
                }
                catch (Exception ex)
                {
                    NLogManager.PublishException(ex);
                }
            }

            return input;
        }

        public static string RemoveBadLinks(string input, out bool Flag)
        {
            input = CutOff(input, " '`~");
            input = ReplaceVN(input);

            Flag = false;
            int bwLength = BadLinks.Count;
            for (int i = 0; i < bwLength; i++)
            {
                try
                {
                    string bl = BadLinks[i];
                    if (bl.StartsWith("regex::", StringComparison.OrdinalIgnoreCase))
                    {
                        bl = bl.Substring(7);
                        Regex regx = new Regex(bl, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                        if (regx.IsMatch(input))
                            Flag = true;
                        input = regx.Replace(input, "*");
                    }
                    else
                    {
                        int countLength = input.Length;
                        input = input.Replace(bl, "*", StringComparison.OrdinalIgnoreCase);
                        if (input.Length != countLength)
                        {
                            Flag = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    NLogManager.PublishException(ex);
                }
            }

            return input;
        }

        public static string RemoveBanUser(string input)
        {
            int bwLength = BanUsers.Count;
            for (int i = 0; i < bwLength; i++)
            {
                try
                {
                    string bw = BanUsers[i];
                    if ( bw == input)
                    {
                        BanUsers.RemoveAt(i);
                        File.WriteAllLines(BANUSERS_FILE, BanUsers);
                        return bw;
                    }
                }
                catch (Exception ex)
                {
                    NLogManager.PublishException(ex);
                }
            }

            return string.Empty;
        }

        public static bool CheckBanUsers(string username)
        {
            return BanUsers.Contains(username);
        }

        public static bool BanUser(string username)
        {
            if(string.IsNullOrEmpty(username))
                return false;

            if (Monitor.TryEnter(lockLoadBanUsers, 60000))
            {
                try
                {
                    if (CheckBanUsers(username)) 
                        return true;

                    File.AppendAllText(BANUSERS_FILE, Environment.NewLine + username);
                    BanUsers.Add(username);
                    NLogManager.LogMessage(string.Format("Admins has been banned user: username={0}", username));
                    return true;
                }
                finally
                {
                    Monitor.Exit(lockLoadBanUsers);
                }
            }
            return false;
        }

        public static bool AddBadLink(string link)
        {
            if (Monitor.TryEnter(lockLoadBadLinks, 60000))
            {
                try
                {
                    File.AppendAllText(BADLINKS_FILE, Environment.NewLine + link);
                    BadLinks.Add(link);
                    NLogManager.LogMessage(string.Format("Admins has been added bad link: link={0}", link));
                    return true;
                }
                finally
                {
                    Monitor.Exit(lockLoadBadLinks);
                }
            }

            return false;
        }

        public static bool AddBadWord(string word)
        {
            if (Monitor.TryEnter(lockLoadBadWords, 60000))
            {
                try
                {
                    File.AppendAllText(BADWORDS_FILE, Environment.NewLine + word);
                    BadWords.Add(word);
                    NLogManager.LogMessage(string.Format("Admins has been added bad word: word={0}", word));
                    return true;
                }
                finally
                {
                    Monitor.Exit(lockLoadBadWords);
                }
            }

            return false;
        }
    }
}