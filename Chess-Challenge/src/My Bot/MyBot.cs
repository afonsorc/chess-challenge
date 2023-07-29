using ChessChallenge.API;
using System;
using System.Diagnostics;


public class MyBot : IChessBot    {
        public struct Entry{
            public ulong key;
            public Move move;
            public int depth, evaluation, evaluationType;
        
            public Entry(ulong key, Move move, int depth, int evaluation, int evaluationType){
                this.key = key;
                this.move = move;
                this.depth = depth;
                this.evaluation = evaluation;
                this.evaluationType = evaluationType;
            }
        }

        const bool TEST_DEPTH = true;
        const int TIME_PER_MOVE = 2000;

        const bool TIMER = !TEST_DEPTH;
        const int MAX_DEPTH = TEST_DEPTH ? 8 : 20; 
        const int ttSize = 8388608;
        public Entry[] transpositionTable = new Entry[ttSize];
        int nNodes = 0;

        // PeSTO Piece Square Table
        readonly int[] pieceValue = { 0, 100, 310, 330, 500, 1000, 0 };
        readonly ulong[,] setPST = {{0, 3617008641903833650, 723412809732590090, 361706447983740165, 86234890240, 358869600189152005, 363113628838791685, 0},{14832572258041583566, 15558810812658215896, 16285027312364814306, 16286440206365557986, 16285032831482003426, 16286434687248368866, 14832572258041583566, 15558810834216938456},{17002766404949505516, 17726168133330272246, 17726173674006183926, 17727581048889738486, 17726179171564650486, 17728993921331759606, 17727575508213826806, 17002766404949505516},{0, 363113758191127045, 18086456103519911931, 18086456103519911931, 18086456103519911931, 18086456103519911931, 18086456103519911931, 21558722560},{17002766426508228076, 17726168133330272246, 17726173652447461366, 18086461622637101051, 18086461622637101056, 17726173652447462646, 17726168133330599926, 17002766426508228076},{16273713057448318946, 16273713057448318946, 16273713057448318946, 16273713057448318946, 16997114785829085676, 17720516557327297526, 1446781380292776980, 1449607125176819220}};
        readonly ulong[,] pestoMiddleGame;
        readonly ulong[,] pestoEndGame;
        Move bestRootMove = Move.NullMove;

        public Move Think(Board board, Timer timer){
            
            // depth should be even to end on opponent's turn 
            nNodes = 0;

            var sw = new Stopwatch();
            //Console.WriteLine((board.PlyCount + 1) / 2);
            int evaluation;
            int alpha = -20000;
            int beta = 20000;
            int window = 100;

            for(int depth = 1; depth <= MAX_DEPTH; depth++){
                sw.Start();
                evaluation = AlphaBetaSearch(board, depth, alpha, beta, timer, 0);
                sw.Stop();

                long ticks = sw.ElapsedTicks;
                int ms = (int)(1000.0 * ticks / Stopwatch.Frequency);
                System.Console.WriteLine("info depth " + depth + " nodes " + nNodes + " time " + ms + " move " + bestRootMove);
/*
                if(evaluation <= alpha || evaluation >= beta){
                    if(evaluation <= alpha) alpha -= 100;
                    if(evaluation >= beta) beta += 100;
                    depth--;
                    System.Console.WriteLine("REPEAT SEARCH");
                }else{
                    alpha = evaluation - window;
                    beta = evaluation + window;
                    System.Console.WriteLine("WORKS");
                }*/

                if(timer.MillisecondsElapsedThisTurn >= timer.MillisecondsRemaining / TIME_PER_MOVE && TIMER)
                    break;
            }

            //System.Console.WriteLine("Stored in TT: " + nTranspositions);
            //System.Console.WriteLine("Found Transposition: " + nLookups);

            //Console.WriteLine(bestMove);
            //Console.WriteLine(move + " " + evaluation);
    /*
            ulong[,] test = new ulong[6, 8];
            for(int rank = 0; rank < 8; rank++){
                for(int file = 0; file < 8; file++){
                    test[0, rank] += (ulong)pstP[rank, file] << (8 * file);
                    test[1, rank] += (ulong)pstN[rank, file] << (8 * file);
                    test[2, rank] += (ulong)pstB[rank, file] << (8 * file);
                    test[3, rank] += (ulong)pstR[rank, file] << (8 * file);
                    test[4, rank] += (ulong)pstQ[rank, file] << (8 * file);
                    test[5, rank] += (ulong)pstK[rank, file] << (8 * file);
                }
            }

            for(int i = 0; i < 6; i++){
                for(int j = 0; j < 8; j++)
                    System.Console.Write(test[i,j] + ", ");
                System.Console.WriteLine();
            }*/




            return bestRootMove == Move.NullMove ? board.GetLegalMoves()[0] : bestRootMove;
        }


        public sbyte GetValuePST(ulong[,] bitboard, int piece, int index){
            return (sbyte)((bitboard[piece, index / 8] >> (56 - (8 * (index % 8)))) & (1 << 8) - 1);;
        }

        public int Evaluate(Board board){

            int evaluation = 0;
            for(var p = 1; p < 6; p++){
                evaluation += pieceValue[p] * (int)board.GetAllPieceLists()[p - 1].Count;
                evaluation -= pieceValue[p] * (int)board.GetAllPieceLists()[p + 5].Count;
            }

            for(var piece = PieceType.Pawn; piece <= PieceType.King; piece++){
                ulong white = board.GetPieceBitboard(piece, true);
                while(white != 0){
                    evaluation += GetValuePST(setPST, (int)piece - 1, BitboardHelper.ClearAndGetIndexOfLSB(ref white) ^ 56);
                }
                ulong black = board.GetPieceBitboard(piece, false);
                while(black != 0){
                   evaluation -= GetValuePST(setPST, (int)piece - 1, BitboardHelper.ClearAndGetIndexOfLSB(ref black) ^ 0);
                }
            }

    /*
            if(!board.HasKingsideCastleRight(true) && !board.HasQueensideCastleRight(true)){
                evaluation -= 150;
            }

            if(!board.HasKingsideCastleRight(false) && !board.HasQueensideCastleRight(false)){
                evaluation += 150;
            }*/
            
            //System.Console.WriteLine("---" + evaluation);

            //if(!board.HasKingsideCastleRight(true)) evaluation -= 50;
            //if(!board.HasQueensideCastleRight(true)) evaluation -= 20;
            //if(!board.HasKingsideCastleRight(false)) evaluation += 50;
            //if(!board.HasQueensideCastleRight(false)) evaluation += 20;
            //System.Console.WriteLine(evaluation);
            return board.IsWhiteToMove ? evaluation : -evaluation;
        }

        public void OrderMoves(Move[] moves, Entry entry){

            int moveCount = moves.Length;
            int[] score = new int[moveCount];

            for(int i = 0; i < moveCount; i++){
                if(moves[i] == entry.move)
                    score[i] = 10000;         
                else if(moves[i].IsCapture){
                    int capture = (int)moves[i].CapturePieceType;
                    int move = (int)moves[i].MovePieceType;
                    score[i] = pieceValue[capture] * capture - pieceValue[move] * move;
                }
                else score[i] = -10000;
            }

            for(int i = 0; i < moveCount - 1; i++){
                int biggestValue = i;
                for(int j = i + 1; j < moveCount; j++)
                    if(score[j] > score[biggestValue]) biggestValue = j;
                if(biggestValue != i){
                    (moves[i], moves[biggestValue]) = (moves[biggestValue], moves[i]);
                    (score[i], score[biggestValue]) = (score[biggestValue], score[i]);
                }
            }
        }

        public int AlphaBetaSearch(Board board, int depth, int alpha, int beta, Timer timer, int ply){

            bool isRoot = ply <= 0;
            //System.Console.WriteLine("PLY " + ply);

            // repetition occurs more often so we check it first
            // might implement penalty for draw or contempt factor
            if(!isRoot && board.IsRepeatedPosition()) return 0;
            if(!isRoot && (board.FiftyMoveCounter >= 100 || board.IsInsufficientMaterial())) return 0;

            ulong zobristHash = board.ZobristKey;
            Entry entry = transpositionTable[zobristHash % ttSize];

            // check if current board position is on transposition table and return it if valid
            if(!isRoot && entry.key == zobristHash && entry.depth >= depth)
                    if(entry.evaluationType == 0 || (entry.evaluationType == 1 && entry.evaluation <= alpha) || (entry.evaluationType == 2 && entry.evaluation >= beta))
                        return entry.evaluation;


            int alphaStart = alpha;
            bool isQueiescenceSearch = depth <= 0;
            int max = -30000;

            if(isQueiescenceSearch){
                max = Evaluate(board);
                //System.Console.WriteLine(max);
                if(max >= beta) return max;
                //int delta = 100;
                //if(standPat < alpha - delta) return alpha;
                if(max > alpha) alpha = max;
            }

            //Check for checkmate or stalemate
            if(board.GetLegalMoves().Length == 0) return board.IsInCheck() ? -10000 : 0;


            Move[] moves = board.GetLegalMoves(isQueiescenceSearch);
            OrderMoves(moves, entry);

            int evaluationType = 0;
            Move bestMove = Move.NullMove;
            foreach(Move move in moves){

                if(timer.MillisecondsElapsedThisTurn >= timer.MillisecondsRemaining / TIME_PER_MOVE && TIMER) return 10000;

                board.MakeMove(move);
                nNodes++;
                int evaluation = -AlphaBetaSearch(board, depth - 1, -beta, -alpha, timer, ply + 1);
                //for(int i = 0; i < ply; i++) System.Console.Write(" ");
                //System.Console.WriteLine(depth + ". " + move + " " + evaluation + " | A: " + alpha + "B: " + beta);
                if(isRoot) System.Console.WriteLine(move + " " + evaluation);
                board.UndoMove(move);

                if(evaluation > max){
                    max = evaluation;
                    bestMove = move;
                    if(isRoot) bestRootMove = move;

                    alpha = Math.Max(alpha, evaluation);
                    if(alpha >= beta) break;
                }
            }
                      
            if(max <= alphaStart) evaluationType = 1;
            else if(max >= beta) evaluationType = 2;

            //if(isRoot) System.Console.WriteLine("Best " + bestRootMove + " " + max);

            transpositionTable[zobristHash % ttSize] = new Entry(zobristHash, bestMove, depth, alpha, evaluationType);
            return max;
        }
    }