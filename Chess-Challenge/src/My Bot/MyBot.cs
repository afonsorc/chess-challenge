using ChessChallenge.API;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.ConstrainedExecution;
using System.Text.RegularExpressions;

public class MyBot : IChessBot{


    public struct Entry{
        public ulong key = 0;
        public Move move = Move.NullMove;
        public int depth = 0;
        public int evaluation = 0;
        public int evaluationType = 0;
    
        public Entry(ulong key, Move move, int depth, int evaluation, int evaluationType){
            this.key = key;
            this.move = move;
            this.depth = depth;
            this.evaluation = evaluation;
            this.evaluationType = evaluationType;
        }
    }

    const bool TIMER = false; 
    const int ttSize = 1000000;
    public Entry[] transpositionTable = new Entry[ttSize];
    Move bestMoveLastIteration = Move.NullMove;
    int nNodes = 0;
    public int nCuts = 0;

    // PeSTO Piece Square Table
    int[] pieceValue = { 0, 82, 337, 365, 477, 1025, 10000 };

    

    public Move Think(Board board, Timer timer){
        
        // depth should be even to end on opponent's turn 
        Move bestMove = Move.NullMove;
        nNodes = 0;

        var sw = new Stopwatch();
        Console.WriteLine((board.PlyCount + 1) / 2);

        int max_depth = 6;
        // Taking 00:39.748s for D7
        for(int depth = 1; depth <= max_depth; depth++){
            sw.Start();
            bestMove = AlphaBetaSearchRoot(board, depth, timer);
            sw.Stop();
            long ticks = sw.ElapsedTicks;
            int ms = (int)(1000.0 * ticks / Stopwatch.Frequency);
            System.Console.WriteLine("info depth " + depth + " nodes " + nNodes + " time " + ms + " move " + bestMove);
            if(timer.MillisecondsElapsedThisTurn >= timer.MillisecondsRemaining / 30 && TIMER)
                break;
        }

        //System.Console.WriteLine("Stored in TT: " + nTranspositions);
        //System.Console.WriteLine("Found Transposition: " + nLookups);

        Console.WriteLine(bestMove);
        //Console.WriteLine(move + " " + evaluation);
/*
        // SET PSTs
        ulong[] setPawn = {0, 3617008641903833650, 723412809732590090, 361706447983740165, 86234890240, 358869600189152005, 363113628838791685, 0};
        ulong[] setKnight = {14832572258041583566, 15558810812658215896, 16285027312364814306, 16286440206365557986, 16285032831482003426, 16286434687248368866, 14832572258041583566, 15558810834216938456};
        ulong[] setBishop = {17002766404949505516, 17726168133330272246, 17726173674006183926, 17727581048889738486, 17726179171564650486, 17728993921331759606, 17727575508213826806, 17002766404949505516};
        ulong[] setRook = {0, 363113758191127045, 18086456103519911931, 18086456103519911931, 18086456103519911931, 18086456103519911931, 18086456103519911931, 21558722560};
        ulong[] setQueen = {17002766426508228076, 17726168133330272246, 17726173652447461366, 18086461622637101051, 18086461622637101056, 17726173652447462646, 17726168133330599926, 17002766426508228076};
        ulong[] setKing = {16273713057448318946, 16273713057448318946, 16273713057448318946, 16273713057448318946, 16997114785829085676, 17720516557327297526, 1446781380292776980, 1449607125176819220};


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

        return bestMove;
    }



    public int Evaluate(Board board){

        int evaluation = 0;
        for(var p = 1; p < 6; p++){
            evaluation += pieceValue[p] * (int)board.GetAllPieceLists()[p - 1].Count;
            evaluation -= pieceValue[p] * (int)board.GetAllPieceLists()[p + 5].Count;
        }

        //ulong bitboard = board.GetPieceBitboard(PieceType.Pawn, true);
        //for(int i = 0; i < BitboardHelper.GetNumberOfSetBits(bitboard); i++){
        //    evaluation += setPawn[];
        //}

        evaluation += EvaluatePawns(board, true) - EvaluatePawns(board, false);
        evaluation += EvaluateKnights(board, true) - EvaluateKnights(board, false);
//

        //ulong bitboard = board.GetPieceBitboard(PieceType.Pawn, true);
        //BitboardHelper.VisualizeBitboard(bitboard);
        //int eval2 = 0;
        //int index = 7 - (number % 8) + (number % 8)
       // System.Console.WriteLine(BitboardHelper.GetNumberOfSetBits(bitboard));
       // for(int i = 0; i < BitboardHelper.GetNumberOfSetBits(bitboard); i++){
       //     System.Console.WriteLine(eval2);
       //     //eval2 += pstPawn[BitboardHelper.ClearAndGetIndexOfLSB(ref bitboard)];
       // }
       // System.Console.WriteLine(eval2);
       // board.GetPieceList(PieceType.Pawn, true);


        return board.IsWhiteToMove ? evaluation : -evaluation;
    }

    // To be replaced by proper PSTs
    public int EvaluatePawns(Board board, bool isWhite){
        
        int evaluation = 0;
        var list = board.GetPieceList(PieceType.Pawn, isWhite);
        for(int p = 0; p < list.Count; p++)
            if(isWhite) evaluation += (list.GetPiece(p).Square.Rank - 1) * 10;
            else evaluation += ( 6 - list.GetPiece(p).Square.Rank) * 10;
        return evaluation;
    }

    // To be replaced by proper PSTs
    public int EvaluateKnights(Board board, bool isWhite){
        
        int evaluation = 0;
        var list = board.GetPieceList(PieceType.Knight, isWhite);
        for(int p = 0; p < list.Count; p++){
            //if ((list.GetPiece(p).Square.Rank == 3 || list.GetPiece(p).Square.Rank == 4) && (list.GetPiece(p).Square.File == 3 || list.GetPiece(p).Square.File == 4)) evaluation += 20;
            if(list.GetPiece(p).Square.Rank == 0 || list.GetPiece(p).Square.Rank == 7 || list.GetPiece(p).Square.File == 0 || list.GetPiece(p).Square.File == 7) evaluation -= 20;
        }
        return evaluation;
    }

    public void SortMoveList(Move[] moves, int[] score){
        
        for(int i = 0; i < moves.Length - 1; i++){
            int biggestValue = i;
            for(int j = i + 1; j < moves.Length; j++)
                if(score[j] > score[biggestValue]) biggestValue = j;
            if(biggestValue != i){
                (moves[i], moves[biggestValue]) = (moves[biggestValue], moves[i]);
                (score[i], score[biggestValue]) = (score[biggestValue], score[i]);
            }
        }
        return;
    }


    public int AlphaBetaSearch(Board board, int depth, int alpha, int beta, Timer timer){

        // Repetition occurs more often so we check it first
        // Might implement penalty for draw or contempt factor
        if(board.IsRepeatedPosition()) return 0;
        if(board.FiftyMoveCounter >= 100 || board.IsInsufficientMaterial()) return 0;

        ulong zobristHash = board.ZobristKey;
        ulong zobristIndex = zobristHash % ttSize;
        Entry entry = transpositionTable[zobristIndex];

        // check if current board position is on transposition table and return it if valid
        if(entry.key == zobristHash)
            if(entry.depth >= depth)
                if(entry.evaluationType == 0 || (entry.evaluationType == 1 && entry.evaluation <= alpha) || (entry.evaluationType == 2 && entry.evaluation >= beta))
                    return entry.evaluation;


        bool isQueiescenceSearch = (depth <= 0);
        if(isQueiescenceSearch){
            int standPat = Evaluate(board);
            if(standPat >= beta){
                transpositionTable[zobristIndex] = new Entry(zobristHash, bestMoveLastIteration, depth, beta, 2);
                return beta;
            }
            //int delta = 100;
            //if(standPat < alpha - delta) return alpha;
            if(standPat > alpha) alpha = standPat;
        }

        Move[] moves = board.GetLegalMoves(isQueiescenceSearch);
        int[] moveScore = new int[moves.Length];

        for(int i = 0; i < moves.Length; i++){
            //System.Console.WriteLine(moves[i]);
            //System.Console.WriteLine(entry.move);
            if(moves[i] == entry.move){
                moveScore[i] = 10000;         
            }
            else if(moves[i].IsCapture) moveScore[i] = (int)moves[i].CapturePieceType - (int)moves[i].MovePieceType;
            else moveScore[i] = -10000;
        }

        SortMoveList(moves, moveScore);

        int evaluationType = 1;
        Move bestMove = Move.NullMove;
        foreach(Move move in moves){

            if(timer.MillisecondsElapsedThisTurn >= timer.MillisecondsRemaining / 30 && TIMER) return 10000;

            board.MakeMove(move);
            nNodes++;
            int evaluation = -AlphaBetaSearch(board, depth - 1, -beta, -alpha, timer);
            //System.Console.WriteLine(depth + ". " + move + " " + evaluation);
            board.UndoMove(move);

            // upper bound beta, opponent will avoid our move
            // lower bound alpha, looking to cut worse moves
            if(evaluation >= beta){
                transpositionTable[zobristIndex] = new Entry(zobristHash, move, depth, beta, 2);
                return beta;
            }
            if(evaluation > alpha){
                evaluationType = 0;
                alpha = evaluation;
                bestMove = move;
            }
        }

        // Check for checkmate or stalemate
        if(board.GetLegalMoves().Length == 0) return board.IsInCheck() ? -10000 : 0;

        transpositionTable[zobristIndex] = new Entry(zobristHash, bestMove, depth, alpha, evaluationType);
        return alpha;
    }


    public Move AlphaBetaSearchRoot(Board board, int depth, Timer timer){

        int max = -30000;
        Move bestMove = Move.NullMove;
        
        foreach (Move move in board.GetLegalMoves()){
            nNodes++;
            board.MakeMove(move);
            int evaluation = -AlphaBetaSearch(board, depth - 1, -10000, 10000, timer);
            board.UndoMove(move);
            //System.Console.WriteLine(depth + ". " + move);

            if (evaluation > max){
                max = evaluation;
                bestMove = move;
                bestMoveLastIteration = move;
            }
           //Console.WriteLine(move + " " + evaluation);
        }
        Console.WriteLine("BEST: " + bestMove + " " + max);
        return bestMove;
    }
}