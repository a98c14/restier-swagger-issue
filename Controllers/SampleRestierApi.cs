using Microsoft.Restier.EntityFrameworkCore;

namespace restier_swagger_bug_report.Controllers;

public partial class SampleRestierApi : EntityFrameworkApi<SampleDbContext>
{
    public SampleRestierApi(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
