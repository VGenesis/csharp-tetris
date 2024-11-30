public class SubmitData{
    private bool submit;
    private int score;

    public class SubmitName{
        private char[] name;
        private int size;

        public SubmitName(int size){
            this.name = new char[size];
            this.size = size;
            for(int i = 0; i < size; i++)
                this.name[i] = ' ';
        }

        public char[] getName(){ return name; }

        public void addChar(char c){
            for(int i = 0; i < this.size-1; i++)
                this.name[i] = this.name[i+1];
            this.name[this.size-1] = c;
        }

        public void delete(){
            for(int i = this.size-1; i > 0; i--)
                this.name[i] = this.name[i-1];
            this.name[0] = ' ';
        }
    };

    public SubmitName name;

    public SubmitData(int size){
        this.submit = false;
        this.score = 0;
        this.name = new SubmitName(size);
    }

    public int getScore() {return score;}
    public void setScore(int score) {this.score = score;}

    public bool isSubmitted() {return submit;}
    public void setSubmitted(bool submit) {this.submit = submit;}

    public string getName(){
        string name = "";
        foreach(char c in this.name.getName()){
            if(c != ' ') name += c;
        }
        return name;
    }
    public void inputChar(char c){ this.name.addChar(c); }
    public void inputBackspace(){ this.name.delete(); }

};
