using System.Threading.Channels;

namespace Seamstress.Application
{
    public class ImageProcessingJob
    {
        public Dictionary<string, NormalizedProduct> Products { get; set; } = new();
        public HashSet<string> ChangedExternalIds { get; set; } = new();
        public int SalePlatformId { get; set; }
    }

    public class ImageProcessingQueue
    {
        private readonly Channel<ImageProcessingJob> _channel = Channel.CreateUnbounded<ImageProcessingJob>();

        public async ValueTask EnqueueAsync(ImageProcessingJob job)
        {
            await _channel.Writer.WriteAsync(job);
        }

        public async ValueTask<ImageProcessingJob> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _channel.Reader.ReadAsync(cancellationToken);
        }
    }
}
