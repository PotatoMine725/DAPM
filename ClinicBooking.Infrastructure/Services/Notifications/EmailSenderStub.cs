using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClinicBooking.Application.Abstractions.Notifications;
using Microsoft.Extensions.Logging;

namespace ClinicBooking.Infrastructure.Services.Notifications;

/// <summary>
/// Implementation tam thoi cua <see cref="IEmailSender"/>.
/// Chi log ra console, khong gui email that.
/// Se duoc thay bang <c>MailKitEmailSender</c> o Tuan 3 Module 4.
/// </summary>
public sealed class EmailSenderStub : IEmailSender
{
    private readonly ILogger<EmailSenderStub> _logger;

    public EmailSenderStub(ILogger<EmailSenderStub> logger)
    {
        _logger = logger;
    }

    public Task GuiAsync(
        string diaChiNhan,
        string tieuDe,
        string noiDungHtml,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "[STUB] GuiEmail: DiaChi={DiaChi}, TieuDe={TieuDe}",
            diaChiNhan, tieuDe);
        return Task.CompletedTask;
    }
}