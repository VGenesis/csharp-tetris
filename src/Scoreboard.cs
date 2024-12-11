public class Scoreboard{
    // Low-to-high sorted dict
    private SortedDictionary<int, string> scores;

    private class DescendingComparer<T> : IComparer<T> where T : IComparable<T> {
        public int Compare(T? x, T? y) => y.CompareTo(x);
    }

    public Scoreboard(){
        this.scores = new SortedDictionary<int, string>(new DescendingComparer<int>());
    }

    public void add(int score, string name) { this.scores.Add(score, name); }
    public KeyValuePair<int, string> at(int i) => this.scores.ElementAt(scores.Count-i-1);
    public int size() => this.scores.Count;

    public void load(string filePath){
        this.scores = new SortedDictionary<int, string>();
        if(File.Exists(filePath)){
            using (StreamReader sr = File.OpenText(filePath)){
                string? line = sr.ReadLine();
                while(line != null){
                    string[] words = line.Split(":");
                    string name = words[0];
                    if(int.TryParse(words[1], out int score))
                        this.scores.Add(score, name);

                    line = sr.ReadLine();
                }
            }
        }else{
            Console.WriteLine("no file");
        }
    }

    public void save(string filePath){
        using (StreamWriter sw = File.CreateText(filePath)){
            foreach(KeyValuePair<int, string> entry in scores){
                string line = entry.Value+ ":" + entry.Key.ToString();
                sw.WriteLine(line);
            }
        }
    }
}
