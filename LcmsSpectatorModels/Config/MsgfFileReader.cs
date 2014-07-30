﻿using System;
using System.Collections.Generic;
using System.IO;
using InformedProteomics.Backend.Data.Composition;
using InformedProteomics.Backend.Data.Sequence;
using InformedProteomics.Backend.MassSpecData;
using LcmsSpectatorModels.Models;

namespace LcmsSpectatorModels.Config
{
    public class MsgfFileReader: IIdFileReader
    {
        public MsgfFileReader(string tsvFile)
        {
            _tsvFile = tsvFile;
        }

        public IdentificationTree Read(LcMsRun lcms)
        {
            var idTree = new IdentificationTree();
            var file = File.ReadLines(_tsvFile);
            var headers = new Dictionary<string, int>();
            var lineCount = 0;
            foreach (var line in file)
            {
                lineCount++;
                if (lineCount == 1) // first line
                {
                    var parts = line.Split('\t');
                    for (int i = 0; i < parts.Length; i++)
                    {
                        headers.Add(parts[i], i);
                    }
                    continue;
                }
                var idData = CreatePrSm(line, headers, lcms);
                idTree.Add(idData);
            }
            return idTree;
        }

        private PrSm CreatePrSm(string line, Dictionary<string, int> headers, LcMsRun lcms)
        {
            var parts = line.Split('\t');
            var scoreLabel = "IcScore";
            if (!headers.ContainsKey(scoreLabel)) scoreLabel = "MSGFScore";
            var score = Convert.ToDouble(parts[headers[scoreLabel]]);
            var prsm = new PrSm
            {
                Lcms = lcms,
                Scan = Convert.ToInt32(parts[headers["ScanNum"]]),
                Protein = parts[headers["Peptide"]],
                Sequence = Sequence.GetSequenceFromMsGfPlusPeptideStr(parts[headers["Peptide"]]),
                SequenceText = parts[headers["Peptide"]],
                Annotation = parts[headers["Peptide"]],
                Composition = Composition.Parse(parts[headers["Formula"]]).ToString(),
                ProteinName = parts[headers["Protein"]].Split('(')[0],
                ProteinDesc = "",
                ProteinNameDesc = parts[headers["Protein"]],
                Charge = Convert.ToInt32(parts[headers["Charge"]]),
                MatchedFragments = Math.Round(score, 3),
                QValue = Math.Round(Convert.ToDouble(parts[headers["QValue"]]), 4),
                PepQValue = Convert.ToDouble(parts[headers["PepQValue"]]),
            };
            return prsm;
        }

        private readonly string _tsvFile;
    }
}