using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tic_tac_toe
{
    public partial class Form1 : Form
    {
        Button[,] Board = new Button[3, 3]; //[colum,row]
        int turn = 0;
        int diffuclty = -1;
        int moves=0;
        Player[] myPlayers=new Player[2];
        public Form1()
        {
            InitializeComponent();
        }
        struct loc
        {
            public int row, col;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Board[i, j] = new System.Windows.Forms.Button();
                    Board[i, j].Size = new System.Drawing.Size((this.Width)/3, this.Height/3);
                    Board[i, j].Location = new System.Drawing.Point(j*(this.Width/3), (this.Height/3)*i);
                    Board[i, j].Name = "[" + i + "," + j + "]";
                    Board[i, j].TabIndex = i+j;
                    Board[i, j].Text = null;
                    Board[i, j].UseVisualStyleBackColor = true;
                    Board[i, j].FlatStyle = FlatStyle.Popup;
                    Board[i, j].Click += new System.EventHandler(Clicked);
                    Board[i, j].Font= new System.Drawing.Font("Microsoft Sans Serif", 50F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    this.Controls.Add(Board[i,j]);
                }
            }
            for(int i=0;i<2;i++)
            {
                if (i == 0)
                {
                    myPlayers[i] = new Player("X");
                }
                else
                {
                    myPlayers[i] = new Player("O");
                } 
            }
        }

        //checks to see if the board is full
        bool board_full()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (Board[i, j].Text.Equals("")) return false;
                }
            }
            return true;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Board[i, j].Size = new System.Drawing.Size(this.Width / 3, this.Height / 3);
                    Board[i, j].Location = new System.Drawing.Point(j * (this.Width / 3), (this.Height / 3) * i);
                }
            }
        }

        //gets the button that was clicked
        private void get_button(object sender,ref int row,ref int colum )
        {
            bool found = false;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (sender.Equals(Board[i, j])) { row = i; colum = j; found = true; break; }
                }
                if (found) break;
            }
        }

        //handler for when the player selects a button
        private void Clicked(object sender, EventArgs e)
        {
            int r = 0, c = 0;
            get_button(sender, ref r, ref c);
            if (!Board[r, c].Text.Equals("X")&&!Board[r,c].Text.Equals("O"))
            {
                Board[r, c].Text = myPlayers[turn].get_letter();
                if (myPlayers[turn].check_if_won(Board))
                {
                   myPlayers[turn].add_win();
                    if (turn == 1) myPlayers[0].add_lose();
                    else myPlayers[1].add_lose();
                    clear_Board();
                    display_score();
                    return;
                }
                if (turn == 1) turn = 0;
                else turn++;
                moves++;
            }
            if (board_full())
            {
                myPlayers[0].add_tie();
                myPlayers[1].add_tie();
                turn = 0;
                clear_Board();
                display_score();
            }
        }

        //clear board
        private void clear_Board()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Board[i, j].Text = null;
                }
            }
            moves = 0;
        }

        //displays score
        private void display_score()
        {
            MessageBox.Show("X's score: " + myPlayers[0].ToString() + "\nO's score: " + myPlayers[1].ToString());
        }

        //selects a random move
        private void random_move()
        {
            Random rand = new Random();
            int r, c;
            if (turn == 1)
            {
                r = rand.Next(3);
                c = rand.Next(3);
                if (!Board[r, c].Text.Equals("X") && !Board[r, c].Text.Equals("O"))
                {
                    Board[r, c].Text = myPlayers[turn].get_letter();
                    if (myPlayers[turn].check_if_won(Board)) 
                    { 
                       myPlayers[turn].add_win();
                        if (turn == 1) myPlayers[0].add_lose();
                        else myPlayers[1].add_lose();
                        //turn = 0;
                        clear_Board();
                        display_score();
                        return;
                    }
                    if (turn == 1) turn = 0;
                    else turn++;
                }
            }
        }

        //intermediate diffuclty
        //checks to see if player or computer has 2 in a row
        //if player has two in a row block player if possible
        //if computer has two in a row take space to win the game if possible
        //other wise take a random space
        private void intermediate()
        {
            if (turn == 1)
            {
                bool[,] oppents_space = new bool[3, 3];
                bool[,] my_space = new bool[3, 3];
                check_player(ref oppents_space, myPlayers[0].get_letter());
                check_player(ref my_space, myPlayers[1].get_letter());
                loc move = get_move_location(my_space, oppents_space);
                //check for wining space
                if (move.col != -1 && move.row != -1)
                {
                    Board[move.row, move.col].Text = myPlayers[turn].get_letter();
                    if (myPlayers[turn].check_if_won(Board))
                    {
                        myPlayers[turn].add_win();
                        if (turn == 1) myPlayers[0].add_lose();
                        else myPlayers[1].add_lose();
                        clear_Board();
                        display_score();
                        return;
                    }
                    if (turn == 1) turn = 0;
                    else turn++;
                }
                else
                {
                    //check for block
                    move = get_move_location(oppents_space, my_space);
                    if (move.col != -1 && move.row != -1)
                    {
                        Board[move.row, move.col].Text = myPlayers[turn].get_letter();
                        if (myPlayers[turn].check_if_won(Board))
                        {
                            myPlayers[turn].add_win();
                            if (turn == 1) myPlayers[0].add_lose();
                            else myPlayers[1].add_lose();
                            clear_Board();
                            display_score();
                            return;
                        }
                        if (turn == 1) turn = 0;
                        else turn++;
                    }
                    else random_move();
                }
            }
        }

        //get players location(s) of moves 
        private void check_player(ref bool[,] player,string let)
        {
            for (int col = 0; col < 3; col++)
            {
                for (int row = 0; row < 3; row++)
                {
                    if (Board[row, col].Text.Equals(let)) player[row, col] = true;
                    else player[row, col] = false;
                }
            }
        }

        //gets a move location based on if the oppoenet has 3 in a row and the last space is open
        //other wise return -1 if there are no moves
        private loc get_move_location(bool[,] oppenent, bool[,] player)
        {
            loc temp;
            temp.row = temp.col = -1;
            //start at top left
            if (oppenent[0, 0])
            {
                //check row
                if (oppenent[0, 1] && !player[0, 2] && !oppenent[0,2])
                {
                    temp.col = 2;
                    temp.row = 0;
                    return temp;
                }
                if (oppenent[0, 2] && !player[0, 1] && !oppenent[0, 1])
                {
                    temp.col = 1;
                    temp.row = 0;
                    return temp;
                }

                //check column
                if (oppenent[1, 0] && !player[2, 0] && !oppenent[2, 0])
                {
                    temp.col = 0;
                    temp.row = 2;
                    return temp;
                }
                if (oppenent[2, 0] && !player[1, 0] && !oppenent[1, 0])
                {
                    temp.col = 0;
                    temp.row = 1;
                    return temp;
                }

                //check Diagonal
                if (oppenent[1, 1] && !player[2, 2] && !oppenent[2, 2])
                {
                    temp.col = temp.row = 2;
                    return temp;
                }
                if (oppenent[2, 2] && !player[1, 1] && !oppenent[1, 1])
                {
                    temp.col = temp.row = 1;
                    return temp;
                }
            }

            //start at top middle
            if (oppenent[0, 1])
            {
                //check row
                if (oppenent[0, 2] && !player[0, 0] && !oppenent[0, 0])
                {
                    temp.row = temp.col = 0;
                    return temp;
                }

                //check column
                if(oppenent[1,1] && !player[2,1] && !oppenent[2,1])
                {
                    temp.col = 1;
                    temp.row = 2;
                    return temp;
                }

                if (oppenent[2, 1] && !player[1, 1] && !oppenent[1, 1])
                {
                    temp.col = temp.row = 1;
                    return temp;
                }
            }

            //start top right corner
            if (oppenent[0, 2])
            {
                //check column
                if (oppenent[1, 2] && !player[2, 2] && !oppenent[2, 2])
                {
                    temp.row = temp.col = 2;
                    return temp;
                }

                if (oppenent[2, 2] && !player[1, 2] && !oppenent[1, 2])
                {
                    temp.col = 2;
                    temp.row = 1;
                    return temp;
                }
                
                //check diagonal
                if (oppenent[1, 1] && !player[2, 0] && !oppenent[2, 0])
                {
                    temp.col = 0;
                    temp.row = 2;
                    return temp;
                }
                if (oppenent[2, 0] && !player[1, 1] && !oppenent[1, 1])
                {
                    temp.col = temp.row = 1;
                    return temp;
                }
            }

            //start left middle row
            if (oppenent[1, 0])
            {
                //check row
                if (oppenent[1, 1] && !player[1, 2] && !oppenent[1, 2])
                {
                    temp.col = 2;
                    temp.row = 1;
                    return temp;
                }
                if (oppenent[1, 2] && !player[1, 1] && !oppenent[1, 1])
                {
                    temp.row = temp.col = 1;
                    return temp;
                }

                // check column
                if (oppenent[2, 0] && !player[0, 0] && !oppenent[0, 0])
                {
                    temp.row = temp.col = 0;
                    return temp;
                }
            }

            if (oppenent[1, 1])
            {
                //check row
                if (oppenent[1, 2] && !player[1, 0] && !oppenent[1, 0])
                {
                    temp.row = 1;
                    temp.col = 0;
                    return temp;
                }
                //check column
                if (oppenent[2, 1] && !player[0, 1] && !oppenent[0, 1])
                {
                    temp.col = 1;
                    temp.row = 0;
                    return temp;
                }

                //check diagonals
                if (oppenent[2, 2] && !player[0, 0] && !oppenent[0, 0])
                {
                    temp.row = temp.col = 0;
                    return temp;
                }

                if (oppenent[2, 0] && !player[0, 2] && !oppenent[0, 2])
                {
                    temp.row = 0;
                    temp.col = 2;
                    return temp;
                }
            }
            
            //check middle right
            if (oppenent[1, 2])
            {
                //check column
                if (oppenent[2, 2] && !player[0, 2] && !oppenent[0, 2])
                {
                    temp.row = 0;

                    temp.col = 2;
                    return temp;
                }
            }

            if (oppenent[2, 0])
            {
                //check row
                if (oppenent[2, 1] && !player[2, 2] && !oppenent[2, 2])
                {
                    temp.col = temp.row = 2;
                    return temp;
                }

                if (oppenent[2, 2] && !player[2, 1] && !oppenent[2, 1])
                {
                    temp.row = 2;
                    temp.col = 1;
                    return temp;
                }


            }
            if (oppenent[2, 1])
            {
                if (oppenent[2, 2] && !player[2, 0] && !oppenent[2, 0])
                {
                    temp.row = 2;
                    temp.col = 0;
                    return temp;
                }
            }
            return temp;
        }


        //choses a random corner
        private loc chose_random_corner()
        {
            loc move;
            Random r = new Random();
            int row, col;
            do
            {
                row = r.Next(3);
                col = r.Next(3);
            } while (row == 1 || col==1);
            move.row = row;
            move.col = col;
            return move;
        }

        //choses a random middle side
        private loc chose_random_side()
        {
            loc move;
            int  col;
            Random r = new Random();
            move.row = 1;
            col = r.Next(2);
            if (col == 0) move.col = col;
            else move.col = 2;
            return move;
        }


        //checks to see if the opposite corner is avaliable
        private bool check_opposite_corner(ref bool[,] player, ref bool [,] computer)
        {
            if (computer[0, 0] && !player[2, 2]) return true;
            if (computer[2, 2] && !player[0, 0]) return true;
            if (computer[0, 2] && !player[2, 0]) return true;
            if (computer[2, 0] && !player[0, 2]) return true;
            return false;
        }

        //makes moves based on player to stop for forks
        private void expert()
        {
            bool[,] player = new bool[3, 3];
            bool[,] computer = new bool[3, 3];
             loc move;
            check_player(ref player,myPlayers[0].get_letter());
            check_player(ref computer, myPlayers[1].get_letter());
            if (turn == 1)
            {
                switch (moves)
                {
                        //if computer starts take corner
                    case (0):
                        move = chose_random_corner();
                        Board[move.row, move.col].Text = myPlayers[turn].get_letter();
                        moves++;
                        break;

                        //if player starts take center if player takes corner or take a random corner
                    case (1):
                        if ((player[0, 0] || player[2, 2] || player[0, 2] || player[2, 0]))
                        {
                            if (!player[1, 1] && !computer[1, 1]) Board[1, 1].Text = myPlayers[turn].get_letter();
                        }
                        else
                        {
                                move = chose_random_corner();
                                if (!player[move.row, move.col] && !computer[move.row, move.col]) Board[move.row, move.col].Text = myPlayers[turn].get_letter();
                        }
                        moves++;
                        break;

                        //computers second turn if computer starts
                        //take opposite corner if avaliavbel otherwise take a random corner
                    case (2):
                        {
                            if (check_opposite_corner(ref player, ref computer))
                            {
                                if (computer[0, 0] && !player[2, 2]) Board[2, 2].Text = myPlayers[turn].get_letter();
                                if (computer[2, 2] && !player[0, 0]) Board[0, 0].Text = myPlayers[turn].get_letter();
                                if (computer[0, 2] && !player[2, 0]) Board[2, 0].Text = myPlayers[turn].get_letter();
                                if (computer[2, 0] && !player[0, 2]) Board[0, 2].Text = myPlayers[turn].get_letter();
                            }
                            else
                            {
                                do
                                {
                                    move = chose_random_corner();
                                } while (computer[move.row, move.col] || player[move.row, move.col]);
                                Board[move.row, move.col].Text = myPlayers[turn].get_letter();
                            }
                            moves++;
                        }
                        break;

                        //player starts computer second turn computer takes a side middle if player takes opposite corners
                        //other wise check for a block
                        //move to block or take a corner or take center
                    case(3):
                        if (((player[0, 0] && player[2, 2]) || (player[0, 2] && player[2, 0])))
                        {
                            move = chose_random_side();
                            if (!player[move.row, move.col] && !computer[move.row, move.col]) Board[move.row, move.col].Text = myPlayers[turn].get_letter();
                        }
                        else
                        {
                            move = get_move_location(player, computer);
                            if (move.row != -1 && move.col != -1)
                            {
                                Board[move.row, move.col].Text = myPlayers[turn].get_letter();
                            }
                            else
                            {
                                if (!computer[1, 1] && !player[1, 1]) Board[1, 1].Text = myPlayers[turn].get_letter();
                                else
                                {
                                   do
                                   {
                                       move = chose_random_corner();
                                   }while (computer[move.row, move.col] || player[move.row, move.col]);
                                    Board[move.row, move.col].Text = myPlayers[turn].get_letter();
                                }
                            }

                        }
                        moves++;
                        break;
                    default:
                        int turnbefore = turn;
                        intermediate();
                        turn = turnbefore;
                        break;

                }
                if (turn == 1) turn = 0;
                else turn++;
            }
        }

        //computer constantly checking to see if it can move
        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (diffuclty)
            {
                case (0):
                    random_move();
                    break;
                case(1):
                    intermediate();
                    break;
                case(2):
                    expert();
                    break;
            }
            if (board_full())
            {
                myPlayers[0].add_tie();
                myPlayers[1].add_tie();
                turn = 0;
                clear_Board();
                display_score();
            }
        }

        //sets computer idle for player vs player and restarts all stats
        private void playerVsPlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            turn = 0;
            diffuclty = -1;
            clear_Board();
            myPlayers[0].clear_score();
            myPlayers[1].clear_score();
        }

        //sets computer to random and resets game
        private void begginerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            turn = 0;
            diffuclty = 0;
            clear_Board();
            myPlayers[0].clear_score();
            myPlayers[1].clear_score();
        }

        //sets computer to intermediate diffuclty and resets game
        private void intermediateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            turn = 0;
            diffuclty = 1;
            clear_Board();
            myPlayers[0].clear_score();
            myPlayers[1].clear_score();
        }

        //sets computer to expert diffuctly and resets game
        private void expertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            turn = 0;
            diffuclty = 2;
            clear_Board();
            myPlayers[0].clear_score();
            myPlayers[1].clear_score();
            moves = 0;
        }
    }
}

//player class that holds:
//player wins, losses, and ties
//the players letter "X" or "O"
//checks to see if player won a match
class Player
{
    private string let;
    private int win, lose, tie;
    public Player(string letter)
    {
        let = letter;
        win = lose = tie = 0;
    }

    public Player(Player copy)
    {
        this.let = copy.let;
        this.win = copy.win;
        this.lose = copy.lose;
        this.tie = copy.tie;
    }

    public string get_letter()
    {
        return let;
    }

    public bool check_if_won(Button[,] Board)
    {
        //top accross
        if (Board[0, 0].Text.Equals(let) && Board[0, 1].Text.Equals(let) && Board[0, 2].Text.Equals(let))
        {

            return true;
        }
        //middle accross
        if (Board[1, 0].Text.Equals(let) && Board[1, 1].Text.Equals(let) && Board[1, 2].Text.Equals(let))
        {
            return true;
        }
        //bottom accross
        if (Board[2, 0].Text.Equals(let) && Board[2, 1].Text.Equals(let) && Board[2, 2].Text.Equals(let))
        {
            return true;
        }
        //left down
        if (Board[0, 0].Text.Equals(let) && Board[1, 0].Text.Equals(let) && Board[2, 0].Text.Equals(let))
        {
            return true;
        }
        //middle down
        if (Board[0, 1].Text.Equals(let) && Board[1, 1].Text.Equals(let) && Board[2, 1].Text.Equals(let))
        {
            return true;
        }
        //left down
        if (Board[0, 2].Text.Equals(let) && Board[1, 2].Text.Equals(let) && Board[2, 2].Text.Equals(let))
        {
            return true;
        }
        //top-left to bottom right diagnol
        if (Board[0, 0].Text.Equals(let) && Board[1, 1].Text.Equals(let) && Board[2, 2].Text.Equals(let))
        {
            return true;
        }
        //bottom-left to top right diagnol
        if (Board[2, 0].Text.Equals(let) && Board[1, 1].Text.Equals(let) && Board[0, 2].Text.Equals(let))
        {
            return true;
        }
        return false;
    }

    public void add_win()
    {
        win++;
    }

    public void add_lose()
    {
        lose++;
    }

    public void add_tie()
    {
        tie++;
    }

    public override String ToString() 
    {
        return ("\nWins: " + win + "\tLoses: " + lose + "\tTies: " + tie + "\n");
    }

    public void clear_score()
    {
        win = lose = tie = 0;
    }
}
