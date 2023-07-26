using ChessChallenge.API;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

public class MyBot : IChessBot{

    /*
    sbyte[] pstPawn = 
    {
        0,  0,  0,  0,  0,  0,  0,  0,
        50, 50, 50, 50, 50, 50, 50, 50,
        10, 10, 20, 30, 30, 20, 10, 10,
        5,  5, 10, 25, 25, 10,  5,  5,
        0,  0,  0, 20, 20,  0,  0,  0,
        5, -5,-10,  0,  0,-10, -5,  5,
        5, 10, 10,-20,-20, 10, 10,  5,
        0,  0,  0,  0,  0,  0,  0,  0
    };

    sbyte[] pstKnight = 
    {
        -50,-40,-30,-30,-30,-30,-40,-50,
        -40,-20,  0,  0,  0,  0,-20,-40,
        -30,  0, 10, 15, 15, 10,  0,-30,
        -30,  5, 15, 20, 20, 15,  5,-30,
        -30,  0, 15, 20, 20, 15,  0,-30,
        -30,  5, 10, 15, 15, 10,  5,-30,
        -40,-20,  0,  5,  5,  0,-20,-40,
        -50,-40,-30,-30,-30,-30,-40,-50,
    };

    sbyte[] pstBishop = 
    {
        -20,-10,-10,-10,-10,-10,-10,-20,
        -10,  0,  0,  0,  0,  0,  0,-10,
        -10,  0,  5, 10, 10,  5,  0,-10,
        -10,  5,  5, 10, 10,  5,  5,-10,
        -10,  0, 10, 10, 10, 10,  0,-10,
        -10, 10, 10, 10, 10, 10, 10,-10,
        -10,  5,  0,  0,  0,  0,  5,-10,
        -20,-10,-10,-10,-10,-10,-10,-20,
    };

    sbyte[] pstRook = 
    {
        0,  0,  0,  0,  0,  0,  0,  0,
        5, 10, 10, 10, 10, 10, 10,  5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        0,  0,  0,  5,  5,  0,  0,  0
    };

    sbyte[] pstQueen = 
    {
        0,  0,  0,  0,  0,  0,  0,  0,
        5, 10, 10, 10, 10, 10, 10,  5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        -5,  0,  0,  0,  0,  0,  0, -5,
        0,  0,  0,  5,  5,  0,  0,  0
    };

    sbyte[] pstKing = 
    {
        -30,-40,-40,-50,-50,-40,-40,-30,
        -30,-40,-40,-50,-50,-40,-40,-30,
        -30,-40,-40,-50,-50,-40,-40,-30,
        -30,-40,-40,-50,-50,-40,-40,-30,
        -20,-30,-30,-40,-40,-30,-30,-20,
        -10,-20,-20,-20,-20,-20,-20,-10,
        20, 20,  0,  0,  0,  0, 20, 20,
        20, 30, 10,  0,  0, 10, 30, 20
    };

    sbyte[] pstKingEnd = 
    {
        -50,-40,-30,-20,-20,-30,-40,-50,
        -30,-20,-10,  0,  0,-10,-20,-30,
        -30,-10, 20, 30, 30, 20,-10,-30,
        -30,-10, 30, 40, 40, 30,-10,-30,
        -30,-10, 30, 40, 40, 30,-10,-30,
        -30,-10, 20, 30, 30, 20,-10,-30,
        -30,-30,  0,  0,  0,  0,-30,-30,
        -50,-30,-30,-30,-30,-30,-30,-50
    };*/


    public struct Entry{
        public ulong key = 0;
        public Move move = Move.NullMove;
        public int depth = 0;
        public int evaluation = 0;
        public int evaluationType = 0;
    
        public Entry(ulong key, int depth, int evaluation, int evaluationType){
            this.key = key;
            this.move = Move.NullMove;
            this.depth = depth;
            this.evaluation = evaluation;
            this.evaluationType = evaluationType;
        }
    }

    public Entry[] transpositionTable = new Entry[1000000];
    int nTranspositions = 0;
    int nLookups = 0;
    Move bestMoveLastIteration = Move.NullMove;
    int nNodes = 0;
    public int nCuts = 0;

    // PeSTO Piece Square Table
    int[] pieceValue = { 0, 82, 337, 365, 477, 1025, 10000 };

    

    public Move Think(Board board, Timer timer){
        
        // depth should be even to end on opponent's turn 
        nTranspositions = 0;
        nLookups = 0;
        Move bestMove = Move.NullMove;
        nNodes = 0;
        nCuts = 0; 

        var sw = new Stopwatch();
        Console.WriteLine((board.PlyCount + 1) / 2);

        int max_depth = 6;
        // Taking 00:39.748s for D7
        for(int depth = 1; depth <= max_depth; depth++){
            sw.Start();
            bestMove = AlphaBetaSearchRoot(board, depth);
            sw.Stop();
            long ticks = sw.ElapsedTicks;
            int ms = (int)(1000.0 * ticks / Stopwatch.Frequency);
            System.Console.WriteLine("info depth " + depth + " nodes " + nNodes + " time " + ms + " move " + bestMove);
        }

        //B: Run stuff you want timed

        //System.Console.WriteLine("Stored in TT: " + nTranspositions);
        //System.Console.WriteLine("Found Transposition: " + nLookups);

        Console.WriteLine(bestMove);
        //Console.WriteLine(move + " " + evaluation);
        return bestMove;
    }



    public int Evaluate(Board board){

        int evaluation = 0;
        for(var p = 0; p < 5; p++){
            evaluation += pieceValue[p] * (int)board.GetAllPieceLists()[p].Count;
            evaluation -= pieceValue[p] * (int)board.GetAllPieceLists()[p + 6].Count;
        }

        evaluation += EvaluatePawns(board, true) - EvaluatePawns(board, false);
        evaluation += EvaluateKnights(board, true) - EvaluateKnights(board, false);


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
            if(isWhite) evaluation += (list.GetPiece(p).Square.Rank - 1) * 5;
            else evaluation += ( 6 - list.GetPiece(p).Square.Rank) * 5;
        return evaluation;
    }

    // To be replaced by proper PSTs
    public int EvaluateKnights(Board board, bool isWhite){
        
        int evaluation = 0;
        var list = board.GetPieceList(PieceType.Knight, isWhite);
        for(int p = 0; p < list.Count; p++){
            //if ((list.GetPiece(p).Square.Rank == 3 || list.GetPiece(p).Square.Rank == 4) && (list.GetPiece(p).Square.File == 3 || list.GetPiece(p).Square.File == 4)) evaluation += 20;
            if(list.GetPiece(p).Square.Rank == 0 || list.GetPiece(p).Square.Rank == 7 || list.GetPiece(p).Square.File == 0 || list.GetPiece(p).Square.File == 7) evaluation -= 30;
        }
        return evaluation;
    }



    public Span<Move> MoveOrdering(Span<Move> moves){

        int[] moveScore = new int[moves.Length];
        for(int i = 0; i < moves.Length; i++){
            
            if(moves[i] == bestMoveLastIteration) moveScore[i] = 10000;
            else if(moves[i].IsCapture){
                moveScore[i] = pieceValue[(int)moves[i].CapturePieceType] - pieceValue[(int)moves[i].MovePieceType];
            }
            else moveScore[i] = -5000;
        }
        SortMoveList(moves, moveScore);
        return moves;
    }

    public void SortMoveList(Span<Move> moves, int[] score){
        
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


    public int AlphaBetaSearch(Board board, int depth, int alpha, int beta){

        ulong zobristHash = board.ZobristKey;
        ulong zobristIndex = zobristHash % 1000000;
        bool isQueiescenceSearch = (depth <= 0);

        // Repetition occurs more often so we check it first
        // Might implement penalty for draw or contempt factor
        if(board.IsRepeatedPosition()) return 0;
        if(board.FiftyMoveCounter >= 100 || board.IsInsufficientMaterial()) return 0;


        // check if current board position is on transposition table and store it if it's empty
        if(transpositionTable[zobristIndex].key == zobristHash){
            if(transpositionTable[zobristIndex].depth >= depth){
                nLookups++;
                return transpositionTable[zobristIndex].evaluation;
            }
        }


        if(isQueiescenceSearch){
            int standPat = Evaluate(board);
            if(standPat >= beta) return beta;
            if(standPat > alpha) alpha = standPat;
        }

        // Check for checkmate or stalemate
        Move[] moves = board.GetLegalMoves(isQueiescenceSearch);
        if(board.GetLegalMoves().Length == 0) return board.IsInCheck() ? -10000 : 0;

        int evaluationType = 1;
        foreach(Move move in MoveOrdering(moves)){
            board.MakeMove(move);
            if(!isQueiescenceSearch)nNodes++;
            //System.Console.Write("  ");
            //ulong positionKey = board.ZobristKey % 100000;
            int evaluation = -AlphaBetaSearch(board, depth - 1, -beta, -alpha);
            //System.Console.WriteLine(depth + ". " + move + " " + evaluation);
            board.UndoMove(move);

            // upper bound beta, opponent will avoid our move
            // lower bound alpha, looking to cut worse moves
            if(evaluation >= beta){
                nTranspositions++;
                transpositionTable[zobristIndex] = new Entry(zobristHash, depth, beta, 2);
                return beta;
            }
            if(evaluation > alpha){
                evaluationType = 0;
                alpha = evaluation;
            }
        }
        nTranspositions++;
        transpositionTable[zobristIndex] = new Entry(zobristHash, depth, alpha, evaluationType);
        return alpha;
    }


    public Move AlphaBetaSearchRoot(Board board, int depth){

        int max = -30000;
        Move bestMove = Move.NullMove;
        
        foreach (Move move in MoveOrdering(board.GetLegalMoves())){
            nNodes++;
            board.MakeMove(move);
            int evaluation = -AlphaBetaSearch(board, depth - 1, -10000, 10000);
            board.UndoMove(move);
            //System.Console.WriteLine(depth + ". " + move);

            if (evaluation > max){
                max = evaluation;
                bestMove = move;
                bestMoveLastIteration = move;
            }
           //Console.WriteLine(move + " " + evaluation);
        }
        return bestMove;
    }
}