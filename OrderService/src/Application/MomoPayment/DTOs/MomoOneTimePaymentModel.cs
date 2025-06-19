﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildingBlocks.Domain.Common;

namespace OrderService.Application.MomoPayment.DTOs;
public class MomoOneTimePaymentModel
{
    public string? PartnerCode { get; set; } // Mã đối tác được cấp bởi MoMo

    public string? RequestId { get; set; } // Guid chuỗi, dùng để phân biệt từng request (request duy nhất)

    public string? OrderId { get; set; } // Guid chuỗi, mã đơn hàng (unique)

    public long Amount { get; set; } // Số tiền thanh toán (đơn vị: VND)

    public string? OrderInfo { get; set; } // Thông tin đơn hàng (tên khách hàng + mô tả)

    public string? RedirectUrl { get; set; } // URL redirect sau khi thanh toán thành công

    public string? IpnUrl { get; set; } // URL MoMo gọi để thông báo kết quả thanh toán (notify URL)

    public string? RequestType { get; set; } = "captureWallet"; // Kiểu yêu cầu: mặc định là captureWallet

    public string? ExtraData { get; set; } // Dữ liệu bổ sung: lưu thông tin order detail dạng mã hóa base64/json

    public string? Lang { get; set; } = "vi"; // Ngôn ngữ giao diện MoMo

    public string? Signature { get; set; } // SHA256 hash dùng để xác thực request



    public void MakeSignature(string accessKey, string secretKey)
    {
        var rawHash = "accessKey=" + accessKey +
               "&amount=" + this.Amount +
               "&extraData=" + this.ExtraData +
               "&ipnUrl=" + this.IpnUrl +
               "&orderId=" + this.OrderId +
               "&orderInfo=" + this.OrderInfo +
               "&partnerCode=" + this.PartnerCode +
               "&redirectUrl=" + this.RedirectUrl +
               "&requestId=" + this.RequestId +
               "&requestType=" + this.RequestType;
        this.Signature = HashHelper.HmacSHA256(rawHash, secretKey);
    }
}

