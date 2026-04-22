using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicBooking.Application.Abstractions.Notifications;

/// <summary>
/// Abstraction gui email — tach biet khoi business logic.
/// Module 4 se implement bang MailKit o Tuan 3.
/// Hien tai dung <see cref="EmailSenderStub"/> de log thay the.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Gui email don gian.
    /// </summary>
    /// <param name="diaChiNhan">Dia chi email nguoi nhan.</param>
    /// <param name="tieuDe">Tieu de email.</param>
    /// <param name="noiDungHtml">Noi dung HTML.</param>
    Task GuiAsync(
        string diaChiNhan,
        string tieuDe,
        string noiDungHtml,
        CancellationToken cancellationToken = default);
}