﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdentificationTree.cs" company="Pacific Northwest National Laboratory">
//   2015 Pacific Northwest National Laboratory
// </copyright>
// <author>Christopher Wilkins</author>
// <summary>
//   A class containing a hierarchy of Protein-Spectrum-Match identifications.
//   The hierarchy is: Protein, Proteoform, Charge state, ID
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InformedProteomics.Backend.MassSpecData;

namespace LcmsSpectator.Models
{
    /// <summary>
    /// A class containing a hierarchy of Protein-Spectrum-Match identifications.
    /// The hierarchy is: Protein, Proteoform, Charge state, ID
    /// </summary>
    public class IdentificationTree : IIdData
    {
        /// <summary>
        /// All possible proteins in the list.
        /// </summary>
        private readonly Dictionary<string, ProteinId> allProteins;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentificationTree"/> class.
        /// </summary>
        /// <param name="tool">The type of tool used for the identifications.</param>
        public IdentificationTree(ToolType tool = ToolType.Other)
        {
            Tool = tool;
            Proteins = new Dictionary<string, ProteinId>();
            allProteins = new Dictionary<string, ProteinId>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentificationTree"/> class.
        /// </summary>
        /// <param name="ids">The identifications to insert into the tree.</param>
        /// <param name="tool">The type of tool used for the identifications.</param>
        public IdentificationTree(IEnumerable<PrSm> ids, ToolType tool = ToolType.Other) : this(tool)
        {
            Add(ids);
        }

        /// <summary>
        /// Gets a dictionary mapping protein name to protein ID.
        /// </summary>
        public Dictionary<string, ProteinId> Proteins { get; }

        /// <summary>
        /// Gets or sets the type of tool used for the identifications.
        /// </summary>
        public ToolType Tool { get; set; }

        /// <summary>
        /// Gets a list of all protein IDs.
        /// </summary>
        public IEnumerable<ProteinId> ProteinIds => from protein in Proteins.Values
                                                    where protein.Sequence.Count != 0
                                                    select protein;

        /// <summary>
        /// Gets a list of all PRSMs
        /// </summary>
        public List<PrSm> AllPrSms => (from protein in Proteins.Values
                                       from proteoform in protein.Proteoforms.Values
                                       from charge in proteoform.ChargeStates.Values
                                       from prsm in charge.PrSms.Values
                                       select prsm).ToList();

        /// <summary>
        /// Gets a list of PRSMs that have a sequence identified
        /// </summary>
        public List<PrSm> IdentifiedPrSms => (from protein in Proteins.Values
                                              from proteoform in protein.Proteoforms.Values
                                              from charge in proteoform.ChargeStates.Values
                                              from prsm in charge.PrSms.Values
                                              where prsm.Sequence.Count > 0
                                              select prsm).ToList();

        /// <summary>
        /// Add a Protein-Spectrum-Match identification.
        /// </summary>
        /// <param name="id">Protein-Spectrum-Math to add</param>
        public void Add(PrSm id)
        {
            RemoveUnidentifiedScan(id);

            if (!allProteins.ContainsKey(id.ProteinName))
            {
                return;
                ////this.allProteins.Add(id.ProteinName, new ProteinId(id.Sequence, id.ProteinName));
            }

            if (!Proteins.ContainsKey(id.ProteinName))
            {
                Proteins.Add(id.ProteinName, allProteins[id.ProteinName]);
            }

            var protein = Proteins[id.ProteinName];
            protein.Add(id);
        }

        /// <summary>
        /// Add a collection of Protein-Spectrum-Match identifications to the tree.
        /// </summary>
        /// <param name="prsms">Collection of Protein-Spectrum-Math identifications.</param>
        public void Add(IEnumerable<PrSm> prsms)
        {
            foreach (var prsm in prsms)
            {
                Add(prsm);
            }
        }

        /// <summary>
        /// Build IdentificationTree from set of PRSMs asynchronously.
        /// </summary>
        /// <param name="prsms">The PRSMs to build the ID tree.</param>
        /// <returns>The task to build tree.</returns>
        public Task BuildIdTree(IEnumerable<PrSm> prsms)
        {
            return Task.Run(() => Add(prsms));
        }

        /// <summary>
        /// Add a new ProteinID from a FASTA Entry.
        /// </summary>
        /// <param name="fastaEntry">The FASTA entry to add.</param>
        public void AddFastaEntry(FastaEntry fastaEntry)
        {
            if (!allProteins.ContainsKey(fastaEntry.ProteinName))
            {
                allProteins.Add(fastaEntry.ProteinName, new ProteinId(fastaEntry));
            }
        }

        /// <summary>
        /// Add a collection of FASTA Entries.
        /// </summary>
        /// <param name="fastaEntry">Collection of FASTA entries.</param>
        public void AddFastaEntries(IEnumerable<FastaEntry> fastaEntry)
        {
            foreach (var entry in fastaEntry)
            {
                AddFastaEntry(entry);
            }
        }

        /// <summary>
        /// Asynchronously add FASTA Entries
        /// </summary>
        /// <param name="fastaEntries">The FASTA entries to add.</param>
        /// <returns>The task.</returns>
        public Task AddFastaEntriesAsync(IEnumerable<FastaEntry> fastaEntries)
        {
            return Task.Run(() => AddFastaEntries(fastaEntries));
        }

        /// <summary>
        /// Insert an existing IDTree into this IDTree.
        /// </summary>
        /// <param name="idTree">IDTree to insert</param>
        public void Add(IdentificationTree idTree)
        {
            Add(idTree.AllPrSms);
        }

        /// <summary>
        /// Remove a Protein-Spectrum-Match identification from the tree.
        /// </summary>
        /// <param name="id">Protein-Spectrum-Match to remove.</param>
        public void Remove(PrSm id)
        {
            if (Proteins.ContainsKey(id.ProteinName))
            {
                Proteins[id.ProteinName].Remove(id);
            }
        }

        /// <summary>
        /// Set the LCMSRun for all data in this IDTree.
        /// </summary>
        /// <param name="lcms">LCMSRun to set.</param>
        /// <param name="dataSetName">Name of the data this for the LCMSRun.</param>
        public void SetLcmsRun(ILcMsRun lcms, string dataSetName)
        {
            foreach (var protein in Proteins.Values)
            {
                protein.SetLcmsRun(lcms, dataSetName);
            }
        }

        /// <summary>
        /// Remove unidentified scans given a PRSM ID containing protein name, proteoform, and scan number.
        /// </summary>
        /// <param name="data">PRSM to remove.</param>
        public void RemoveUnidentifiedScan(PrSm data)
        {
            if (Proteins.TryGetValue(string.Empty, out var protein)
                && protein.Proteoforms.TryGetValue(string.Empty, out var proteoform)
                && proteoform.ChargeStates.TryGetValue(0, out var chargeState))
            {
                chargeState.Remove(data);
            }
        }

        /// <summary>
        /// Clear all PRSMs.
        /// </summary>
        public void ClearIds()
        {
            foreach (var proteinId in allProteins)
            {
                proteinId.Value.ClearIds();
            }

            ClearEmptyProteins();
        }

        /// <summary>
        /// Clear all proteins without proteoforms.
        /// </summary>
        public void ClearEmptyProteins()
        {
            Proteins.Clear();
            foreach (var protein in allProteins)
            {
                if (protein.Value.Proteoforms.Count > 0)
                {
                    Proteins.Add(protein.Key, protein.Value);
                }
            }
        }

        /// <summary>
        /// Determines whether the IDTree contains a given identification.
        /// </summary>
        /// <param name="id">the ID to search for.</param>
        /// <returns>A value indicating whether the IDTree contains the identification.</returns>
        public bool Contains(PrSm id)
        {
            return Proteins.Values.Any(protein => protein.Contains(id));
        }

        /// <summary>
        /// Get the PRSM in the tree with the highest score.
        /// </summary>
        /// <returns>The PRSM with the highest score.</returns>
        public PrSm GetHighestScoringPrSm()
        {
            PrSm highest = null;
            foreach (var protein in Proteins.Values)
            {
                var highestProtein = protein.GetHighestScoringPrSm();
                if (highest == null || highestProtein.CompareTo(highest) >= 0)
                {
                    highest = highestProtein;
                }
            }

            return highest;
        }

        /// <summary>
        /// Get a protein associated with a certain ID.
        /// </summary>
        /// <param name="id">ID to search for.</param>
        /// <returns>The protein ID found.</returns>
        public ProteinId GetProtein(PrSm id)
        {
            ProteinId protein = null;
            if (Proteins.ContainsKey(id.ProteinName))
            {
                protein = Proteins[id.ProteinName];
            }

            return protein;
        }

        /// <summary>
        /// Get a proteoform associated with a certain ID.
        /// </summary>
        /// <param name="id">ID to search for.</param>
        /// <returns>The proteoform ID found.</returns>
        public ProteoformId GetProteoform(PrSm id)
        {
            var protein = GetProtein(id);
            if (protein == null)
            {
                return null;
            }

            ProteoformId proteoform = null;
            if (protein.Proteoforms.ContainsKey(id.SequenceText))
            {
                proteoform = protein.Proteoforms[id.SequenceText];
            }

            return proteoform;
        }

        /// <summary>
        /// Get a charge state associated with a certain ID.
        /// </summary>
        /// <param name="id">ID to search for.</param>
        /// <returns>The charge state ID found.</returns>
        public ChargeStateId GetChargeState(PrSm id)
        {
            var proteoform = GetProteoform(id);
            if (proteoform == null)
            {
                return null;
            }

            ChargeStateId chargeState = null;
            if (proteoform.ChargeStates.ContainsKey(id.Charge))
            {
                chargeState = proteoform.ChargeStates[id.Charge];
            }

            return chargeState;
        }

        /// <summary>
        /// Get PRSMs with the same Protein Name, Sequence, Charge, and Scan number
        /// </summary>
        /// <param name="id">PRSM object to search for</param>
        /// <returns>PRSM found in tree with parameters specified by data</returns>
        public PrSm GetPrSm(PrSm id)
        {
            var chargeState = GetChargeState(id);
            if (chargeState == null)
            {
                return null;
            }

            PrSm prsm = null;
            if (chargeState.Contains(id))
            {
                prsm = chargeState.PrSms[new Tuple<int, string>(id.Scan, id.RawFileName)];
            }

            return prsm;
        }
    }
}
