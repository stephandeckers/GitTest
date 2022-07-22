namespace PXS.IfRF.Data.Model
{
    public class PositionConnectionSearchResponse
    {
        public SrPosition SubrackPosition { get; set; }
        public SrPosition RelatedPosition1 { get; set; }
        public SrPosition RelatedPosition2 { get; set; }
        public Connection ConnectionInfo { get; set; }

    }
}
