window.GlobalHeader = window.GlobalHeader || {};

(function () {
    GlobalHeader.Config = {
        Root_Url: window.location.origin + '/'
    };

    GlobalHeader.init = function (data) {
        this.IsLogin = data.IsLogin;
        this.LoginUrl = data.LoginUrl;
        this.LogOutUrl = data.LogOutUrl;
        this.render(data);
    }

    GlobalHeader.keyPressDoubleNumber = function ($this, eve) {
        if ((eve.which != 46 || $(this).val().indexOf('.') != -1) && (eve.which < 48 || eve.which > 57) || (eve.which == 46 && $($this).caret().start == 0)) {
            eve.preventDefault();
        }
    }

    GlobalHeader.keyupDouble = function ($this, eva) {
        if ($($this).val().indexOf('.') == 0) {
            $($this).val($($this).val().substring(1));
        }
    }
    GlobalHeader.AgencyRefund = function () {
        $sId = $("#sID").val();
        var inputData = { ServiceID: $sId };
        if (!confirm("Thu hồi tiền gift code của đại lý?")) {
            return;
        }

        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Agency/AgencyRefund",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',

            success: function (data) {
                console.log(data)
                if (data.ResponseCode == 1) {
                    alert(data.Message);

                } else {
                    alert(data.Message);
                }
                location.reload();
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }
    GlobalHeader.AgencyChagneWalletType = function () {
        $Type = $('input[name=walletType]:checked').val();
        if ($Type == 1) {
            $Amount = $("#CurrentWallet").val($('#WalletAmount').val());
        }
        if ($Type == 3) {
            $Amount = $("#CurrentWallet").val($('#GiftcodeWalletAmount').val());
        }
    }

    GlobalHeader.TruQuy = (gameid, roomid, idCong = false) => {
        data = prompt("Vui lòng nhập số quỹ cần " + (idCong ? 'Cộng' : 'Trừ'), "");
        var inputData = { Gameid: gameid, Roomid: roomid, moneyde: idCong ? -data : data };

        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Game/TruQuy",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',

            success: function (data) {
                console.log(data)
                if (data.ResponseCode == 1) {
                    alert(data.Message);

                } else {
                    alert(data.Message);
                }
                location.reload();
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }

    GlobalHeader.UpdateRateJP = (roomid) => {
        data = prompt("Vui lòng nhập tỷ lệ mới", "");
        var inputData = { Roomid: roomid, Rate: Number(data) };

        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Setting/UpdateRateJP",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',

            success: function (data) {
                console.log(data)
                if (data.ResponseCode == 1) {
                    alert(data.Message);

                } else {
                    alert(data.Message);
                }
                location.reload();
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }


    GlobalHeader.AgencyWalletWithDraw = function () {

        $Type = $('input[name=walletType]:checked').val();

        if ($Type != 1 && $Type != 3) {
            alert(" Bạn chưa chọn loại ví ");
            return;
        }
        $Amount = $("#AmountRefund").val().trim().replace(/\./g, '');
        if ($Amount === null || $Amount.length <= 0) {
            alert("Bạn chưa nhập số tiền thu hồi")
            return;
        }
        if ($Amount <= 0) {
            alert("Giá trị thu hồi phải lớn hơn 0")
            return;
        }
        if ($Type == 1) {
            $wallet = $('#WalletF').val();
            if (Number($Amount) > Number($wallet)) {
                alert("Số tiền muốn rút lớn hơn số tiền trong VÍ CHÍNH")
                return;
            }
        }
        if ($Type == 3) {
            $wallet2 = $('#GiftcodeWalletF').val();

            if (Number($Amount) > Number($wallet2)) {
                alert("Số tiền muốn rút lớn hơn số tiền trong VÍ GIFT CODE")
                return;
            }
        }
        $Note = $("#NoteRefund").val();
        if ($Note === null || $Note.length <= 0) {
            alert("Bạn chưa nhập lý do thu hồi")
            return;
        }
        $sID = $("#SIDTH").val();
        $AgencyID = $("#AgID").val();

        var msg = "Thu hồi  tiền trong VÍ CHÍNH của đại lý?"
        if ($Type == 3) {
            msg = "Thu hồi tiền trong VÍ GIFT CODE của đại lý?"
        }
        if (!confirm(msg)) {
            return;
        }

        var inputData = { ServiceID: $sID, Type: $Type, Amount: $Amount, Note: $Note, AgencyID: $AgencyID };


        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Agency/AgencyRefundEachOther",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',

            success: function (data) {
                console.log(data)
                if (data.ResponseCode == 1) {
                    alert(data.Message);

                } else {
                    alert(data.Message);
                }
                location.reload();
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }


    GlobalHeader.ChooseGiftCodeType = function (type) {
        if (type == 1) {
            $("#agencyTr").hide();
            $("#UserTr").hide();
        } else if (type == 2) {

            $("#agencyTr").show();
            $("#UserTr").hide();
        } else if (type == 3) {

            $("#agencyTr").hide();
            $("#UserTr").show();
        }
    };

    GlobalHeader.ConfirmDelete = function () {
        return confirm("Bạn có chắc chắn muốn xóa dữ liệu bản ghi này");
    };

    GlobalHeader.InputNumber = function (event) {
        return event.charCode >= 48 && event.charCode <= 57 || event.key === "Backspace"
    };

    GlobalHeader.IsNumberKey = function (evt) {
        var charCode = (evt.which) ? evt.which : event.keyCode
        if (charCode > 31 && (charCode < 48 || charCode > 57))
            return false;

        return true;
    }

    GlobalHeader.closeLoginOtp = function () {
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "CustomerSupport/CloseLoginOtp",
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (data) {
                var msg = "Tắt đăng nhập otp " + (data == 1 ? "thành công" : "thất bại")
                GlobalHeader.notifyMessage(msg, data);
                if (data == 1) {
                    $('#disableSecure').remove();
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.validateNickName = function (type) {
        var nickname = $("#nickname").val().trim();
        if (nickname == '') {
            GlobalHeader.dataValMsgFor('nickName', 'Vui lòng nhập tài khoản!');
            $('#nickname').focus();
            return;
        }
        var inputData = { nickName: nickname, accountType: type };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Admin/ValidateNickName",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (data) {
                if (data <= 0) {
                    GlobalHeader.dataValMsgFor('nickName', 'Tài khoản không tồn tại!');
                    $('#nickname').focus();
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.validateUserTransfer = function (type) {
        var receiverName = $("#receiverName").val().trim();
        var ServiceID = $("#ServiceID").val().trim();
        if (receiverName == '') {
            GlobalHeader.dataValMsgFor('receiverName', 'Vui lòng nhập tài khoản nhận!');
            $('#receiverName').focus();
            return;
        }
        var inputData = { nickName: receiverName, accountType: type, ServiceID: ServiceID };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Admin/ValidateNickName",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {
                if (data < 0) {
                    if (data == -105) {
                        GlobalHeader.dataValMsgFor('receiverName', 'Tài khoản không hợp lệ');
                    } else {
                        GlobalHeader.dataValMsgFor('receiverName', 'Tài khoản không tồn tại');
                    }
                    $('#receiverName').focus();
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.validateUserTransferPL = function (type) {
        var receiverNameLst = $("#receiverNameList").val().trim();
        var receiverName = $("#receiverName").val().trim();
        if (receiverName == '' && receiverNameLst == '') {
            GlobalHeader.dataValMsgFor('receiverName', 'Vui lòng nhập tài khoản nhận!');
            $('#receiverName').focus();
            return;
        }

        if (receiverName == '' && receiverNameLst != null)
            return;

        var inputData = { nickName: receiverName, accountType: type };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Admin/ValidateNickName",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {
                if (data < 0) {
                    if (data == -105) {
                        GlobalHeader.dataValMsgFor('receiverName', 'Tài khoản không hợp lệ');
                    } else {
                        GlobalHeader.dataValMsgFor('receiverName', 'Tài khoản không tồn tại');
                    }
                    if (receiverNameLst == '') {
                        $('#receiverName').focus();
                    }
                } else if (data == 1) {
                    receiverNameLst += receiverName + ',';
                    $("#receiverNameList").val(receiverNameLst);
                    $('#receiverName').val('').focus();
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.GetXoSoResult = function ($ketQuaDate) {



        var inputData = { KetQuaDate: $ketQuaDate };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "XoSo/GetResultInfor",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {
                $("#GiaiDB").val(data.GiaiDB);
                $("#GiaiNhat").val(data.GiaiNhat);
                $("#GiaiNhiMot").val(data.GiaiNhiMot);
                $("#GiaiNhiHai").val(data.GiaiNhiHai);

                $("#GiaiBaMot").val(data.GiaiBaMot);
                $("#GiaiBaHai").val(data.GiaiBaHai);
                $("#GiaiBaBa").val(data.GiaiBaBa);
                $("#GiaiBaBon").val(data.GiaiBaBon);
                $("#GiaiBaNam").val(data.GiaiBaNam);
                $("#GiaiBaSau").val(data.GiaiBaSau);

                $("#GiaiBonMot").val(data.GiaiBonMot);
                $("#GiaiBonHai").val(data.GiaiBonHai);
                $("#GiaiBonBa").val(data.GiaiBonBa);
                $("#GiaiBonBon").val(data.GiaiBonBon);

                $("#GiaiNamMot").val(data.GiaiNamMot);
                $("#GiaiNamHai").val(data.GiaiNamHai);
                $("#GiaiNamBa").val(data.GiaiNamBa);
                $("#GiaiNamBon").val(data.GiaiNamBon);
                $("#GiaiNamNam").val(data.GiaiNamNam);
                $("#GiaiNamSau").val(data.GiaiNamSau);

                $("#GiaiSauMot").val(data.GiaiSauMot);
                $("#GiaiSauHai").val(data.GiaiSauHai);
                $("#GiaiSauBa").val(data.GiaiSauBa);



                $("#GiaiBayMot").val(data.GiaiBayMot);
                $("#GiaiBayHai").val(data.GiaiBayHai);
                $("#GiaiBayBa").val(data.GiaiBayBa);
                $("#GiaiBayBon").val(data.GiaiBayBon);



            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.addReceiverNameSpecial = function () {
        var receiverNameLst = $("#receiverNameList").val().trim();
        var receiverName = $("#receiverName").val().trim();
        if (receiverName == '' && receiverNameLst == '') {
            GlobalHeader.dataValMsgFor('receiverName', 'Vui lòng nhập tài khoản nhận!');
            $('#receiverName').focus();
            return;
        }

        if (receiverName == '' && receiverNameLst != null)
            return;

        var inputData = { nickName: receiverName };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Admin/ValidateNickNameSpecial",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {
                if (data < 0) {
                    if (data == -105) {
                        GlobalHeader.dataValMsgFor('receiverName', 'Tài khoản không hợp lệ');
                    } else {
                        GlobalHeader.dataValMsgFor('receiverName', 'Tài khoản không tồn tại');
                    }
                    if (receiverNameLst == '') {
                        $('#receiverName').focus();
                    }
                } else if (data == 1) {
                    receiverNameLst += receiverName + ',';
                    $("#receiverNameList").val(receiverNameLst);
                    $('#receiverName').val('').focus();
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.validateAgencyTransfer = function () {
        var receiverName = $("#receiverName").val().trim();
        var ServiceID = $("#ServiceID").val().trim();
        if (receiverName == '') {
            GlobalHeader.dataValMsgFor('receiverName', 'Vui lòng nhập tài khoản nhận!');
            $('#receiverName').focus();
            return;
        }
        var inputData = { nickName: receiverName, ServiceID: ServiceID };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Admin/GetReceicerAgencyInfo",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {
                if (data.Response < 0) {
                    if (data.Response == -105) {
                        GlobalHeader.dataValMsgFor('receiverName', 'Tài khoản không hợp lệ!');
                    } else if (data.Response == -101) {
                        GlobalHeader.dataValMsgFor('receiverName', 'Bạn không thể chuyển cho chính bạn!');
                    } else {
                        GlobalHeader.dataValMsgFor('receiverName', 'Tài khoản không tồn tại!');
                    }
                    $('#receiverName').focus();
                }
                if (data.AccountLevel == 2) {
                    if ($("#walletType option[value='3']").length > 0)
                        $("#walletType option[value='3']").remove();
                } else {
                    if (!($("#walletType option[value='3']").length > 0)) {
                        $('#walletType').append($('<option>', {
                            value: 3,
                            text: 'Ví giftcode'
                        }));
                    }
                }
                $('#txttransfee').text(data.FeePercent);
                $('#transfee').val(data.Fee);
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.validateAdminTransfer = function () {
        var receiverName = $("#receiverName").val().trim();
        if (receiverName == '') {
            GlobalHeader.dataValMsgFor('receiverName', 'Vui lòng nhập tài khoản nhận!');
            $('#receiverName').focus();
            return;
        }
        var inputData = { nickName: receiverName };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Admin/GetAdminIDByNickName",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {
                if (data <= 0) {
                    GlobalHeader.dataValMsgFor('receiverName', 'Tài khoản không tồn tại!');
                    $('#receiverName').focus();
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.validateAdminTestTransfer = function () {
        var receiverName = $("#receiverName").val().trim();
        if (receiverName == '') {
            GlobalHeader.dataValMsgFor('receiverName', 'Vui lòng nhập tài khoản nhận!');
            $('#receiverName').focus();
            return;
        }
        var inputData = { nickName: receiverName };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Admin/GetAdminTestIDByNickName",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {
                if (data <= 0) {
                    GlobalHeader.dataValMsgFor('receiverName', 'Tài khoản không tồn tại!');
                    $('#receiverName').focus();
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.changeRecvMoney = function (e) {
        var amount = e.value.replace(/\./g, '');
        var orgAmount = parseInt(amount) * (1 + parseFloat($('#fee').val()));
        $('#orgAmount').val(GlobalHeader.formatNumber(orgAmount));
    };

    GlobalHeader.TransDescription = function ($this) {
        $val = $($this).val();
        if ($val == 1) {
            $("#trasdes").html("Trạng thái thành công người mua Bit sẽ được xử thắng trong tranh chấp")
        } else if ($val == 4) {
            $("#trasdes").html("Trạng thái hủy giao dịch người bán Bit sẽ được xử thắng trong tranh chấp")
        }
    }

    GlobalHeader.TransProcess = function () {
        if (!confirm("Cập nhật trạng thái của giao dịch")) return false;
        $TransID = $("#TransID").val();
        $RequestStatus = $("#RequestStatus").val();
        $ComplainID = $("#ComplainID").val();
        var inputData = { TransID: $TransID, RequestStatus: $RequestStatus, ComplainID: $ComplainID };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Complain/UserMarketUpdate",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {
                if (data.ResponseCode == 1) {
                    window.location = "/Complain/ComplainEdit/" + $ComplainID + "?active=2"
                } else {
                    alert(data.Message);
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.ProcessRequest = function ($no) {
        $CreateResult = $("#CreateResult_" + $no).val();
        $ReceiveResult = $("#ReceiveResult_" + $no).val();
        $ComplainID = $("#ComplainID").val();
        if ($CreateResult == '' || $CreateResult.length <= 0) {
            alert('Nhập quá trình xử lý người bán lần ' + $no);
            $("#CreateResult_" + $no).focus();
            return;
        }
        if ($ReceiveResult == '' || $ReceiveResult.length <= 0) {
            alert('Nhập quá trình xử lý người mua lần ' + $no);
            $("#ReceiveResult_" + $no).focus();
            return;
        }

        var inputData = { CreateResult: $CreateResult, ReceiveResult: $ReceiveResult, ComplainID: $ComplainID, No: $no };

        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Complain/ComplainProcess",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',

            success: function (data) {
                if (data.ResponseCode == 1) {
                    window.location = "/Complain/ComplainEdit/" + $ComplainID + "?active=1"

                } else {
                    alert(data.Message);
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.validationFormTransfer = function (type) {
        var wallet = parseInt($("#wallet").text().trim().replace(/\./g, ''));
        var receiverName = $("#receiverName").val().trim();
        var amount = $("#amount").val().trim();
        var orgAmount = $("#orgAmount").val().trim();
        var note = $("#note").val().trim();
        var iamount = parseInt(amount.replace(/\./g, ''));
        var iorgAmount = parseInt(orgAmount.replace(/\./g, ''));

        if (receiverName == null || receiverName == '') {
            GlobalHeader.dataValMsgFor('receiverName', 'Vui lòng nhập tài khoản nhận!')
            $("#receiverName").focus();
            return false;
        }

        if (amount == '' || amount == null) {
            GlobalHeader.dataValMsgFor('amount', 'Vui lòng nhập số nhận!');
            $("#amount").focus();
            return false;
        }

        if (iamount < 10000) {
            GlobalHeader.dataValMsgFor('amount', 'Số tiền nhận tối thiểu là 10.000!');
            $("#amount").focus();
            return false;
        }

        if (iorgAmount > wallet) {
            GlobalHeader.dataValMsgFor('amount', 'Số tiền thực chuyển lớn hơn số dư hiện có!');
            $("#amount").focus();
            return false;
        }

        if (type == 2) {
            var walletType = $("#walletType").val().trim();
            if (walletType != 1 && walletType != 3) {
                GlobalHeader.dataValMsgFor('walletType', 'Giá trị ví không hợp lệ!')
                $("#walletType").focus();
                return false;
            }
        }

        if (note == '' || note == null) {
            GlobalHeader.dataValMsgFor('note', 'Vui lòng nhập nội dung!');
            $("#note").focus();
            return false;
        }

        return true;
    };
    GlobalHeader.validationFormSafebalance = function (type) {
        var receiverName = $("#receiverName").val().trim();
        var amount = $("#amount").val().trim();
        var note = $("#note").val().trim();
        var iamount = parseInt(amount.replace(/\./g, ''));

        if (receiverName == null || receiverName == '') {
            GlobalHeader.dataValMsgFor('receiverName', 'Vui lòng nhập tài khoản nhận!')
            $("#receiverName").focus();
            return false;
        }

        if (amount == '' || amount == null) {
            GlobalHeader.dataValMsgFor('amount', 'Vui lòng nhập số nhận!');
            $("#amount").focus();
            return false;
        }

        if (iamount < 10000) {
            GlobalHeader.dataValMsgFor('amount', 'Số tiền nhận tối thiểu là 10.000!');
            $("#amount").focus();
            return false;
        }

        if (note == '' || note == null) {
            GlobalHeader.dataValMsgFor('note', 'Vui lòng nhập nội dung!');
            $("#note").focus();
            return false;
        }

        return true;
    };
    GlobalHeader.validationCampaign = function (type) {
        if (type == 0) {
            var campaignCode = $("#campaignCode").val().trim();
            if (campaignCode == null || campaignCode == '') {
                GlobalHeader.dataValMsgFor('campaignCode', 'Vui lòng nhập mã chiến dịch!')
                $("#campaignCode").focus();
                return false;
            }
        }
        var d = new Date();
        var now = d.getFullYear() + '' + d.getMonth() + 1 + '' + d.getDate();
        var campaignName = $("#campaignName").val().trim();
        var effectDate = $("#effectDate").val().trim();
        var expiredDate = $("#expiredDate").val().trim();
        var description = $("#description").val().trim();

        if (campaignName == null || campaignName == '') {
            GlobalHeader.dataValMsgFor('campaignName', 'Vui lòng nhập tên chiến dịch!')
            $("#campaignName").focus();
            return false;
        }

        if (effectDate == '' || effectDate == null) {
            GlobalHeader.dataValMsgFor('effectDate', 'Vui lòng nhập ngày hiệu lực!');
            $("#effectDate").focus();
            return false;
        }

        if (expiredDate == '' || expiredDate == null) {
            GlobalHeader.dataValMsgFor('expiredDate', 'Vui lòng nhập ngày hết hạn!');
            $("#expiredDate").focus();
            return false;
        }

        if (parseInt(GlobalHeader.formatDateTimehms(effectDate, 7)) <= parseInt(GlobalHeader.getDateNow())) {
            GlobalHeader.dataValMsgFor('effectDate', 'Ngày hiệu lực phải lớn hơn ngày hiện tại!');
            $("#effectDate").focus();
            return false;
        }

        if (parseInt(GlobalHeader.formatDateTimehms(effectDate, 7)) >= parseInt(GlobalHeader.formatDateTimehms(expiredDate, 7))) {
            GlobalHeader.dataValMsgFor('effectDate', 'Ngày hiệu lực phải nhỏ hơn ngày hết hạn!');
            $("#effectDate").focus();
            return false;
        }

        if (description == '') {
            GlobalHeader.dataValMsgFor('description', 'Vui lòng nhập miêu tả!');
            $("#description").focus();
            return false;
        }

        return true;
    }

    GlobalHeader.validationSetEvent = function (type) {
        if (type == 0) {
            var campaignId = $("#campaignId").val().trim();
            var gameId = $("#gameId").val().trim();
            var roomId = $("#roomId").val().trim();
            if (campaignId == 0) {
                GlobalHeader.dataValMsgFor('campaignId', 'Vui lòng chọn chiến dịch!')
                return false;
            }

            if (gameId == 0) {
                GlobalHeader.dataValMsgFor('campaignId', 'Vui lòng chọn game!')
                return false;
            }

            if (roomId == 0) {
                GlobalHeader.dataValMsgFor('roomId', 'Vui lòng chọn phòng!')
                return false;
            }
        }

        var jackpotEventInit = $("#jackpotEventInit").val().trim();
        var jackpotEventQuota = $("#jackpotEventQuota").val().trim();
        var jackpotStepJump = $("#jackpotStepJump").val().trim();
        var eventDay1 = $("#eventDay1").is(":checked");
        var eventDay2 = $("#eventDay2").is(":checked");
        var eventDay3 = $("#eventDay3").is(":checked");
        var eventDay4 = $("#eventDay4").is(":checked");
        var eventDay5 = $("#eventDay5").is(":checked");
        var eventDay6 = $("#eventDay6").is(":checked");
        var eventDay7 = $("#eventDay7").is(":checked");
        var eventTime = $("#eventTime").val().trim();
        var effectDate = $("#effectDate").val().trim();
        var expiredDate = $("#expiredDate").val().trim();
        var description = $("#description").val().trim();
        var MAX_INT = Math.pow(2, 53);

        if (jackpotEventInit == null || jackpotEventInit == '') {
            GlobalHeader.dataValMsgFor('jackpotEventInit', 'Vui lòng nhập giá trị hũ sự kiện khởi tạo!')
            $("#jackpotEventInit").focus();
            return false;
        } else if (jackpotEventInit.replace(/\./g, '') < 1 || jackpotEventInit.replace(/\./g, '') > MAX_INT) {
            GlobalHeader.dataValMsgFor('jackpotEventInit', 'Giá trị nhập vào không hợp lệ!');
            $("#jackpotEventInit").focus();
            return false;
        }

        if (jackpotEventQuota == null || jackpotEventQuota == '') {
            GlobalHeader.dataValMsgFor('jackpotEventQuota', 'Vui lòng nhập số lượng hũ sự kiện!')
            $("#jackpotEventQuota").focus();
            return false;
        } else if (jackpotEventInit.replace(/\./g, '') < 1 || jackpotEventInit.replace(/\./g, '') > MAX_INT) {
            GlobalHeader.dataValMsgFor('jackpotEventQuota', 'Giá trị nhập vào không hợp lệ!');
            $("#jackpotEventQuota").focus();
            return false;
        }

        if (jackpotStepJump == null || jackpotStepJump == '') {
            GlobalHeader.dataValMsgFor('jackpotStepJump', 'Vui lòng nhập bước nhảy hũ sự kiện!')
            $("#jackpotStepJump").focus();
            return false;
        } else if (jackpotStepJump.replace(/\./g, '') < 1 || jackpotStepJump.replace(/\./g, '') > MAX_INT) {
            GlobalHeader.dataValMsgFor('jackpotStepJump', 'Giá trị nhập vào không hợp lệ!');
            $("#jackpotStepJump").focus();
            return false;
        }

        if (!(eventDay1 || eventDay2 || eventDay3 || eventDay4 || eventDay5 || eventDay6 || eventDay7)) {
            GlobalHeader.dataValMsgFor('eventDay', 'Vui lòng nhập ngày sự kiện!');
            return false;
        }

        if (eventTime == '' || eventTime == null) {
            GlobalHeader.dataValMsgFor('eventTime', 'Vui lòng nhập thời gian diễn ra sự kiện!');
            $("#eventTime").focus();
            return false;
        }

        if (effectDate == '' || effectDate == null) {
            GlobalHeader.dataValMsgFor('effectDate', 'Vui lòng nhập thời gian diễn ra sự kiện!');
            $("#effectDate").focus();
            return false;
        }

        if (expiredDate == '' || expiredDate == null) {
            GlobalHeader.dataValMsgFor('expiredDate', 'Vui lòng nhập thời gian diễn ra sự kiện!');
            $("#effectDate").focus();
            return false;
        }

        if (description == '' || description == null) {
            GlobalHeader.dataValMsgFor('eventTime', 'Vui lòng nhập thời gian diễn ra sự kiện!');
            $("#eventTime").focus();
            return false;
        }

        return true;
    }

    GlobalHeader.updateUserToBlacklist = function (userid, status, serviceid) {
        var inputData = { userId: userid, status: status, serviceId: serviceid };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "User/UpdateUserToBlacklist",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (data) {
                GlobalHeader.dataValMsgFor('user-manager', data.Msg);
                if (data.Response == 1) {
                    $('.adminData [data-valmsg-for="user-manager"]').addClass('text-success');
                } else {
                    $('.adminData [data-valmsg-for="user-manager"]').removeClass('text-success');
                }
                setTimeout(function () {
                    $('#btnSearch').click();
                }, 2000);
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }

    GlobalHeader.UpdateBlackList = function (userid, status) {
        var inputData = { userId: userid, status: status };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "User/UpdateUserToBlacklist",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (data) {
                GlobalHeader.dataValMsgFor('user-manager', data.Msg);
                if (data.Response == 1) {
                    alert("Cập nhật balck list thành công")
                } else {
                    alert(data.Msg)
                }
                setTimeout(function () {
                    $('#btnSearch').click();
                }, 2000);
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }

    GlobalHeader.lockGameBai = function (userId, nickname, serviceid) {
        var msg = "";
        msg = "Nhập thông tin KHÓA người dùng GAME BÀI : " + nickname


        var inputmsg = prompt(msg);
        if (inputmsg == null || inputmsg == "") {
            alert("Bạn chưa nhập lý do ");
            return;
        }

        nickname = nickname.trim();
        var inputData = { UserId: userId, NickName: nickname, ServiceID: serviceid, Msg: inputmsg };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Admin/UserGameBaiLock",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (data) {
                GlobalHeader.dataValMsgFor('user-manager', data.Msg);
                if (data.Response == 1) {
                    $('.adminData [data-valmsg-for="user-manager"]').addClass('text-success');
                    $('#btnSearch').click();
                } else {
                    $('.adminData [data-valmsg-for="user-manager"]').removeClass('text-success');
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.lockAndUnlockReset = function (nickname, status, serviceid) {
        var msg = "";
        if (status == 1) {
            msg = "Nhập thông tin MỞ người dùng " + nickname
        }
        if (status == 2) {
            msg = "Nhập thông tin KHÓA người dùng " + nickname
        }
        if (status == 3) {
            msg = "Nhập thông tin RESET PASSWORD người dùng " + nickname
        }

        var inputmsg = prompt(msg);
        if (inputmsg == null || inputmsg == "") {
            alert("Bạn chưa nhập lý do ");
            return;
        }

        nickname = nickname.trim();
        var inputData = { NickName: nickname, Status: status, ServiceID: serviceid, Msg: inputmsg };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Admin/UserManager",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (data) {
                GlobalHeader.dataValMsgFor('user-manager', data.Msg);
                if (data.Response == 1) {
                    $('.adminData [data-valmsg-for="user-manager"]').addClass('text-success');
                    $('#btnSearch').click();
                } else {
                    $('.adminData [data-valmsg-for="user-manager"]').removeClass('text-success');
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.userLockTrans = function (nickname, status, serviceid) {
        nickname = nickname.trim();
        var inputData = { NickName: nickname, Status: status, ServiceID: serviceid };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Admin/UserLockTrans",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (data) {
                GlobalHeader.dataValMsgFor('user-manager', data.Msg);
                if (data.Response == 1) {
                    $('.adminData [data-valmsg-for="user-manager"]').addClass('text-success');
                    $('#btnSearch').click();
                } else {
                    $('.adminData [data-valmsg-for="user-manager"]').removeClass('text-success');
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.banUserChat = function (nickname, status, serviceid) {
        nickname = nickname.trim();
        var inputData = { nickName: nickname, rdoStatus: status, serviceId: serviceid };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Admin/BanUserChat",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (data) {
                GlobalHeader.dataValMsgFor("user-manager", data.Msg);
                if (data.Response == 1) {
                    $('.adminData [data-valmsg-for="user-manager"]').addClass('text-success');
                } else {
                    $('.adminData [data-valmsg-for="user-manager"]').removeClass('text-success');
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.getReportAgencyL1CashFlowUsersDetails = function (currPage) {
        $("#currentPage").val(currPage);
        $("#frmAgencyL1CashFlowUsersDetails").submit();
    };

    GlobalHeader.getHistoryCardExchange = function (currPage) {
        var userName = $('#userName').val().trim();
        var nickName = $('#nickName').val().trim();
        var cardNumber = $('#cardNumber').val().trim();
        var cardSerial = $('#cardSerial').val().trim();
        var cardSerial = $('#cardSerial').val().trim();
        var buyDate = $('#buyDate').val().trim();
        var status = $('#status').val().trim();
        var serviceId = $('#serviceId').val().trim();
        var param = '?userName=' + userName + '&nickName=' + nickName + '&cardNumber=' + cardNumber + '&cardSerial=' + cardSerial
            + '&buyDate=' + buyDate + '&status=' + status + '&currentPage=' + currPage + '&serviceId=' + serviceId;

        $.ajax({
            type: "GET",
            url: GlobalHeader.Config.Root_Url + "Card/GetCardExChangeList" + param,
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (html) {
                $('#hisCardExchange').html(html);
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.getMomoList = function (currPage) {
        var NickName = $('#NickName').val().trim();
        var RequestCode = $('#RequestCode').val().trim();
        var RefKey = $('#RefKey').val().trim();
        var RefSendKey = $('#RefSendKey').val().trim();

        var fromDate = $('#fromDate').val().trim();
        var toDate = $('#toDate').val().trim();
        var type = $('#Type').val().trim();
        var status = $('#status').val().trim();
        var serviceId = $('#serviceId').val().trim();
        var partnerID = $('#partnerID').val().trim();
        var MomoReceive = $('#MomoReceive').val().trim();

        var param = '?NickName=' + NickName + '&RequestCode=' + RequestCode
            + '&RefKey=' + RefKey
            + '&RefSendKey=' + RefSendKey
            + '&FromRequestDate=' + fromDate
            + '&ToRequestDate=' + toDate
            + '&Status=' + status + '&currentPage=' + currPage + '&ServiceID=' + serviceId + '&partnerID=' + partnerID + '&MomoReceive=' + MomoReceive;

        $.ajax({
            type: "GET",
            url: GlobalHeader.Config.Root_Url + "Momo/GetMomoList" + param,
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (html) {
                $('#hisCardExchange').html(html);
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.userMomoSwapExamine = function (userId, requestID, checkStatus) {
        var inputData = { requestID: requestID, userId: userId, checkStatus: checkStatus };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Momo/UserMomoSwapExamine",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (data) {
                if (data.ResponseCode == 1) {
                    GlobalHeader.getMomoListRut(1);
                } else {
                    alert(data.Message);
                    GlobalHeader.getMomoListRut(1);
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };
    GlobalHeader.userMomoChargeExamine = function (userId, requestID, checkStatus) {
        var inputData = { requestID: requestID, userId: userId, checkStatus: checkStatus };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Momo/UserMomoChargeExamine",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (data) {
                if (data.ResponseCode == 1) {
                    GlobalHeader.getMomoListNap(1);
                } else {
                    alert(data.Message);
                    GlobalHeader.getMomoListNap(1);
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };
    GlobalHeader.userMomoSwapManual = function (userId, requestID, checkStatus) {
        var inputData = { requestID: requestID, userId: userId, checkStatus: checkStatus };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Momo/UserMomoSwapManual",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (data) {
                if (data.ResponseCode == 1) {
                    GlobalHeader.getMomoListRut(1);
                } else {
                    alert(data.Message);
                    GlobalHeader.getMomoListRut(1);
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };
    GlobalHeader.getMomoListRut = function (currPage) {
        var NickName = $('#NickName').val().trim();
        var RequestCode = $('#RequestCode').val().trim();
        var RefKey = $('#RefKey').val().trim();
        var RefSendKey = $('#RefSendKey').val().trim();

        var fromDate = $('#fromDate').val().trim();
        var toDate = $('#toDate').val().trim();
        var type = $('#Type').val().trim();
        var status = $('#status').val().trim();
        var serviceId = $('#serviceId').val().trim();
        var partnerID = $('#partnerID').val().trim();
        var MomoReceive = $('#MomoReceive').val().trim();

        var param = '?NickName=' + NickName + '&RequestCode=' + RequestCode
            + '&RefKey=' + RefKey
            + '&RefSendKey=' + RefSendKey
            + '&FromRequestDate=' + fromDate
            + '&ToRequestDate=' + toDate
            + '&Status=' + status + '&currentPage=' + currPage + '&ServiceID=' + serviceId + '&partnerID=' + partnerID + '&MomoReceive=' + MomoReceive;

        $.ajax({
            type: "GET",
            url: GlobalHeader.Config.Root_Url + "Momo/GetMomoListRut" + param,
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (html) {
                $('#hisCardExchange').html(html);
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };
    GlobalHeader.getMomoListNap = function (currPage) {
        var NickName = $('#NickName').val().trim();
        var RequestCode = $('#RequestCode').val().trim();
        var RefKey = $('#RefKey').val().trim();
        var RefSendKey = $('#RefSendKey').val().trim();

        var fromDate = $('#fromDate').val().trim();
        var toDate = $('#toDate').val().trim();
        var type = $('#Type').val().trim();
        var status = $('#status').val().trim();
        var serviceId = $('#serviceId').val().trim();
        var partnerID = $('#partnerID').val().trim();
        var MomoReceive = $('#MomoReceive').val().trim();

        var param = '?NickName=' + NickName + '&RequestCode=' + RequestCode
            + '&RefKey=' + RefKey
            + '&RefSendKey=' + RefSendKey
            + '&FromRequestDate=' + fromDate
            + '&ToRequestDate=' + toDate
            + '&Status=' + status + '&currentPage=' + currPage + '&ServiceID=' + serviceId + '&partnerID=' + partnerID + '&MomoReceive=' + MomoReceive;

        $.ajax({
            type: "GET",
            url: GlobalHeader.Config.Root_Url + "Momo/GetMomoListNap" + param,
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (html) {
                $('#hisCardExchange').html(html);
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };
    GlobalHeader.userBankSwapExamine = function (userId, RequestId, checkStatus) {
        if (checkStatus == -2) //từ chối rút tiền
        {
            let note = prompt("Nhập lí do từ chối rút tiền", "Wrong information!");
            if (note == null) {
                alert("Bạn cần nhập nội dung từ chối!");
                return;
            }
            var inputData = { RequestId: RequestId, userId: userId, checkStatus: checkStatus, note: note };
            $.ajax({
                type: "POST",
                url: GlobalHeader.Config.Root_Url + "BankExchagne/UserUSDTExamineWithNote",
                data: JSON.stringify(inputData),
                contentType: "application/json; charset=utf-8",
                dataType: 'json',
                xhrFields: { withCredentials: true },
                crossDomain: true,
                success: function (data) {
                    if (data.ResponseCode == 1) {
                        GlobalHeader.getBankListRut(1);
                    } else {
                        alert(data.Message);
                        GlobalHeader.getBankListRut(1);
                    }
                },
                fail: function (fail) {
                    console.log("Loi");
                }
            });

        } else { //cho phép rút tiền
            var inputData = { RequestId: RequestId, userId: userId, checkStatus: checkStatus };
            $.ajax({
                type: "POST",
                url: GlobalHeader.Config.Root_Url + "BankExchagne/UserUSDTExamine",
                data: JSON.stringify(inputData),
                contentType: "application/json; charset=utf-8",
                dataType: 'json',
                xhrFields: { withCredentials: true },
                crossDomain: true,
                success: function (data) {
                    if (data.ResponseCode == 1) {
                        GlobalHeader.getBankListRut(1);
                    } else {
                        alert(data.Message);
                        GlobalHeader.getBankListRut(1);
                    }
                },
                fail: function (fail) {
                    console.log("Loi");
                }
            });
        }

    };
    GlobalHeader.userBankSwapManual = function (userId, RequestId, checkStatus) {
        var inputData = { RequestId: RequestId, userId: userId, checkStatus: checkStatus };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "BankExchagne/UserUSDTManual",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (data) {
                if (data.ResponseCode == 1) {
                    GlobalHeader.getBankListRut(1);
                } else {
                    alert(data.Message);
                    GlobalHeader.getBankListRut(1);
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };
    GlobalHeader.getBankListRut = function (currPage) {
        var NickName = $('#NickName').val().trim();
        var RequestCode = $('#RequestCode').val().trim();
        //var RefKey = $('#RefKey').val().trim();
        //var RefSendKey = $('#RefSendKey').val().trim();

        var fromDate = $('#fromDate').val().trim();
        var toDate = $('#toDate').val().trim();
        var type = $('#Type').val().trim();
        var status = $('#status').val().trim();
        var serviceId = $('#serviceId').val().trim();
        //var partnerID = $('#partnerID').val().trim();
        //var MomoReceive = $('#MomoReceive').val().trim();

        var param = '?NickName=' + NickName + '&RequestCode=' + RequestCode
            //+ '&RefKey=' + RefKey
            //+ '&RefSendKey=' + RefSendKey
            + '&FromRequestDate=' + fromDate
            + '&ToRequestDate=' + toDate
            + '&Status=' + status + '&currentPage=' + currPage + '&ServiceID=' + serviceId
        //+ '&partnerID=' + partnerID + '&MomoReceive=' + MomoReceive;

        $.ajax({
            type: "GET",
            url: GlobalHeader.Config.Root_Url + "USDT/GetUSTDListRut" + param,
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (html) {
                $('#hisCardExchange').html(html);
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };
    GlobalHeader.getBankListNap = function (currPage) {
        var NickName = $('#NickName').val().trim();
        var RequestCode = $('#RequestCode').val().trim();
        //var RefKey = $('#RefKey').val().trim();
        //var RefSendKey = $('#RefSendKey').val().trim();

        var fromDate = $('#fromDate').val().trim();
        var toDate = $('#toDate').val().trim();
        var type = $('#Type').val().trim();
        var status = $('#status').val().trim();
        var serviceId = $('#serviceId').val().trim();
        //var partnerID = $('#partnerID').val().trim();
        //var MomoReceive = $('#MomoReceive').val().trim();

        var param = '?NickName=' + NickName + '&RequestCode=' + RequestCode
            //+ '&RefKey=' + RefKey
            //+ '&RefSendKey=' + RefSendKey
            + '&FromRequestDate=' + fromDate
            + '&ToRequestDate=' + toDate
            + '&Status=' + status + '&currentPage=' + currPage + '&ServiceID=' + serviceId
        //+ '&partnerID=' + partnerID + '&MomoReceive=' + MomoReceive;

        $.ajax({
            type: "GET",
            url: GlobalHeader.Config.Root_Url + "USDT/GetUSTDListNap" + param,
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (html) {
                $('#hisCardExchange').html(html);
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };
    GlobalHeader.getSMSRequestList = function (currPage) {




        var NickName = $('#NickName').val().trim();

        var RefKey = $('#RefKey').val().trim();
        var Phone = $('#Phone').val().trim();

        var fromDate = $('#fromDate').val().trim();
        var toDate = $('#toDate').val().trim();

        var status = $('#status').val().trim();
        var serviceId = $('#serviceId').val().trim();
        var partnerID = $('#partnerID').val().trim();


        var param = '?NickName=' + NickName
            + '&RefKey=' + RefKey
            + '&Phone=' + Phone
            + '&FromRequestDate=' + fromDate
            + '&ToRequestDate=' + toDate
            + '&Status=' + status + '&currentPage=' + currPage + '&ServiceID=' + serviceId + '&partnerID=' + partnerID;

        $.ajax({
            type: "GET",
            url: GlobalHeader.Config.Root_Url + "SmsCharge/GetSmsList" + param,
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (html) {
                $('#hisCardExchange').html(html);
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.getUSDTList = function (currPage) {



        var nickName = $('#nickName').val().trim();
        var code = $('#code').val().trim();
        var fromDate = $('#fromDate').val().trim();
        var toDate = $('#toDate').val().trim();
        var type = $('#Type').val().trim();
        var status = $('#status').val().trim();

        var serviceId = $('#serviceId').val().trim();
        var param = '?nickName=' + nickName + '&code=' + code + '&fromDate=' + fromDate
            + '&toDate=' + toDate + '&status=' + status + '&currentPage=' + currPage + '&serviceId=' + serviceId + '&type=' + type;

        $.ajax({
            type: "GET",
            url: GlobalHeader.Config.Root_Url + "USDT/GetUSTDList" + param,
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (html) {
                $('#hisCardExchange').html(html);
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };
    GlobalHeader.getUSDTListBankExchagne = function (currPage) {



        var nickName = $('#NickName').val().trim();
        var code = $('#RequestCode').val().trim();
        var fromDate = $('#fromDate').val().trim();
        var toDate = $('#toDate').val().trim();
        var type = $('#Type').val().trim();
        var status = $('#status').val().trim();

        var serviceId = $('#serviceId').val().trim();
        var param = '?nickName=' + nickName + '&code=' + code + '&fromDate=' + fromDate
            + '&toDate=' + toDate + '&status=' + status + '&currentPage=' + currPage + '&serviceId=' + serviceId + '&type=' + type;

        $.ajax({
            type: "GET",
            url: GlobalHeader.Config.Root_Url + "BankExchagne/GetUSTDList" + param,
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (html) {
                $('#hisCardExchange').html(html);
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.GetUSDTByID = function () {
        $requestId = $("#Check_RequestID").val();
        if ($requestId == null || $requestId.length <= 0) {
            alert("Chưa nhập mã yêu cầu")
        };
        var inputData = { requestId: $requestId };

        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "USDT/GetUSDTByID",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',

            success: function (data) {
                console.log(data)
                if (data.ResponseCode == 1) {

                    $("#Check_NickName").val(data.obj.DisplayName);
                    $("#Check_Type").val(data.obj.RequestName);
                    $("#Check_Code").val(data.obj.RequestCode);

                    $("#Check_Status").val(data.obj.StatusStr);
                    $("#Check_Game_Request").val(data.obj.AmountGame);
                    $("#Check_USDT_Request").val(data.obj.AmountUSDT);
                    $("#Check_VND_Request").val(data.obj.AmountVND);

                    //$("#Check_Status").val(data.obj.StatusStr);

                } else {
                    alert(data.Message);
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }
    GlobalHeader.GetUSDTExchangeByID = function () {
        $requestId = $("#Check_RequestID").val();
        if ($requestId == null || $requestId.length <= 0) {
            alert("Chưa nhập mã yêu cầu")
        };
        var inputData = { requestId: $requestId };

        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "BankExchagne/GetUSDTByID",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',

            success: function (data) {
                console.log(data)
                if (data.ResponseCode == 1) {

                    $("#Check_NickName").val(data.obj.DisplayName);
                    $("#Check_Type").val(data.obj.RequestName);
                    $("#Check_Code").val(data.obj.RequestCode);

                    $("#Check_Status").val(data.obj.StatusStr);
                    $("#Check_Game_Request").val(data.obj.AmountVND);

                    $("#Check_VND_Request").val(data.obj.AmountGame);

                    //$("#Check_Status").val(data.obj.StatusStr);

                } else {
                    alert(data.Message);
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }

    GlobalHeader.USDTCheck = function (type) {
        var msg = "Bạn có muốn hủy giao dịch";
        if (type == 1) {
            msg = "Bạn có muốn hoàn trả lại giao dịch";
        }
        if (!confirm(msg)) {
            return;
        }

        $requestId = $("#Check_RequestID").val();
        $AmountGame = $("#Check_Game_Refund").val();
        $AmountUSDT = $("#Check_USDT_Refund").val();
        $AmountVND = $("#Check_VND_Refund").val();
        $CheckNote = $("#Check_Note").val();
        if (type != 1 && type != 0) {
            if ($requestId == null || $requestId.length <= 0) {
                alert("Tham số type không hợp lệ")
            };
        }
        if ($requestId == null || $requestId.length <= 0) {
            alert("Chưa nhập mã yêu cầu")
        };
        if ($AmountGame === null || $AmountGame.length <= 0) {
            alert("Giá trị mệnh giá Game không hợp lệ")
        }
        if ($AmountGame <= 0) {
            alert("Giá trị mệnh giá Game không hợp lệ")
        }
        if ($AmountUSDT === null || $AmountUSDT.length <= 0) {
            alert("Giá trị mệnh giá USDT không hợp lệ")
        }
        if ($AmountUSDT <= 0) {
            alert("Giá trị mệnh giá USDT không hợp lệ")
        }
        if ($AmountUSDT === null || $AmountUSDT.length <= 0) {
            alert("Giá trị mệnh giá USDT không hợp lệ")
        }
        if ($AmountUSDT <= 0) {
            alert("Giá trị mệnh giá USDT không hợp lệ")
        }
        if ($AmountVND === null || $AmountVND.length <= 0) {
            alert("Giá trị mệnh giá VND không hợp lệ")
        }
        if ($AmountVND <= 0) {
            alert("Giá trị mệnh giá VND không hợp lệ")
        }
        if ($CheckNote === null || $CheckNote.length <= 0) {
            alert("Không có giá trị mô tả")
        }

        var inputData = { requestId: $requestId, AmountGame: $AmountGame, AmountUSDT: $AmountUSDT, AmountVND: $AmountVND, CheckNote: $CheckNote, Type: type };

        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "USDT/CheckUSDT",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',

            success: function (data) {
                console.log(data)
                if (data.ResponseCode == 1) {
                    alert(data.Message);
                    $("#Check_NickName").val("");
                    $("#Check_Type").val("");
                    $("#Check_Code").val("");

                    $("#Check_Status").val("");
                    $("#Check_Game_Request").val("");
                    $("#Check_USDT_Request").val("");
                    $("#Check_VND_Request").val("");
                    $("#Check_RequestID").val("");
                    $("#Check_Game_Refund").val("");
                    $("#Check_USDT_Refund").val("");
                    $("#Check_VND_Refund").val("");
                    $("#Check_Note").val("");
                    GlobalHeader.getUSDTList(1);
                } else {
                    alert(data.Message);
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }
    GlobalHeader.USDTCheckExchagne = function (type) {
        var msg = "Bạn có hoàn tiền giao dịch";
        if (type == 1) {
            msg = "Bạn có muốn cập nhật giao dịch thành công";
        }
        if (!confirm(msg)) {
            return;
        }

        $requestId = $("#Check_RequestID").val();

        $CheckNote = $("#Check_Note").val();
        if (type != 1 && type != 0) {
            if ($requestId == null || $requestId.length <= 0) {
                alert("Tham số type không hợp lệ")
            };
        }
        if ($requestId == null || $requestId.length <= 0) {
            alert("Chưa nhập mã yêu cầu")
        };
        if ($CheckNote === null || $CheckNote.length <= 0) {
            alert("Không có giá trị mô tả")
        }

        var inputData = { requestId: $requestId, CheckNote: $CheckNote, Type: type };

        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "BankExchagne/CheckUSDT",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',

            success: function (data) {
                console.log(data)
                if (data.ResponseCode == 1) {
                    alert(data.Message);
                    $("#Check_NickName").val("");
                    $("#Check_Type").val("");
                    $("#Check_Code").val("");

                    $("#Check_Status").val("");
                    $("#Check_Game_Request").val("");
                    $("#Check_USDT_Request").val("");
                    $("#Check_VND_Request").val("");
                    $("#Check_RequestID").val("");

                    $("#Check_Note").val("");
                    GlobalHeader.getUSDTListBankExchagne(1);
                } else {
                    alert(data.Message);
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }
    GlobalHeader.CalcuatelUSDT = function () {
        $requestId = $("#Check_RequestID").val();
        if ($requestId == null || $requestId.length <= 0) {
            alert("Chưa nhập mã yêu cầu")
        };
        $AmountUSDT = $("#Check_USDT_Refund").val();
        var inputData = { requestId: $requestId, AmountUSDT: $AmountUSDT };

        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "USDT/CalcuatelUSDT",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',

            success: function (data) {
                console.log(data)
                if (data.ResponseCode == 1) {
                    $("#Check_Game_Refund").val(data.AmountGame);
                    $("#Check_VND_Refund").val(data.AmountVND);



                } else {
                    alert(data.Message);
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }
    GlobalHeader.CalcuatelUSDTExchagne = function () {
        $requestId = $("#Check_RequestID").val();
        if ($requestId == null || $requestId.length <= 0) {
            alert("Chưa nhập mã yêu cầu")
        };

        var inputData = { requestId: $requestId };

        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "BankExchagne/CalcuatelUSDT",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',

            success: function (data) {
                console.log(data)
                if (data.ResponseCode == 1) {
                    $("#Check_USDT_Request").val(data.Amount);




                } else {
                    alert(data.Message);
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }


    GlobalHeader.RemovePhoneOtp = function (userid, ServiceID) {
        var inputData = { userId: userid, ServiceID: ServiceID };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "User/RemovePhoneOtp",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (data) {
                GlobalHeader.dataValMsgFor('user-manager', data.Msg);
                if (data.Response == 1) {
                    alert("Xóa số điện thoại thành công")
                } else {
                    alert(data.Msg);
                }
                setTimeout(function () {
                    $('#btnSearch').click();
                }, 2000);
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }
    GlobalHeader.userCardSwapExamine = function (userCardSwap, user, status) {
        var inputData = { userCardSwapId: userCardSwap, userId: user, checkStatus: status };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Card/UserCardSwapExamine",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (data) {
                if (data.ResponseCode == 1) {
                    GlobalHeader.getHistoryCardExchange(1);
                } else {
                    alert(data.Message);
                    GlobalHeader.getHistoryCardExchange(1);
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.userUSDTExamine = function (RequestID, user, status) {
        var inputData = { RequestID: RequestID, userId: user, checkStatus: status };
        var msg = "";
        if (status == 0) {
            msg = "Bạn có muốn HỦY giao dịch rút tiền của khách hàng?";
        } else {
            msg = "Bạn có muốn ĐỒNG Ý giao dịch rút tiền của khách hàng?";
        }
        if (!confirm(msg)) {
            return;
        }
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "BankExchagne/UserUSDTExamine",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (data) {
                if (data.ResponseCode == 1) {
                    alert("Thành công");
                    GlobalHeader.getUSDTListBankExchagne(1);
                } else {
                    alert(data.Message);
                    GlobalHeader.getUSDTListBankExchagne(1);

                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.userBankChargeExamine = function (user, RequestID, status) {
        var inputData = { RequestID: RequestID, userId: user, checkStatus: status };
        var msg = "";
        if (status == -2) {
            msg = "Bạn có muốn HỦY giao dịch nạp tiền của khách hàng?";
        } else {
            msg = "Bạn có muốn ĐỒNG Ý giao dịch nạp tiền của khách hàng?";
        }
        if (!confirm(msg)) {
            return;
        }
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "BankExchagne/UserUSDTChargeExamine",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (data) {
                if (data.ResponseCode == 1) {
                    alert("Thành công");
                    GlobalHeader.getBankListNap(1);
                } else {
                    alert(data.Message);
                    GlobalHeader.getBankListNap(1);

                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.getHistoryCardRechard = function (currPage) {
        var cardNumber = $('#cardNumber').val().trim();
        var cardSerial = $('#cardSerial').val().trim();
        var cardValue = $('#cardValue').val().trim();
        var nickName = $('#nickName').val().trim();
        var telOperatorId = $('#TelOperatorID').val().trim();
        var fromDate = $('#fromDate').val().trim();
        var toDate = $('#toDate').val().trim();
        var status = $('#Status').val().trim();
        var PartnerID = $('#PartnerID').val().trim();
        var PartnerErrorCode = null;
        if ($('#PartnerErrorCode').is(":checked")) {
            PartnerErrorCode = "-99";
        }
        var smg = $('#smg').val().trim();
        var serviceId = $('#serviceId').val().trim();
        var param = '?cardNumber=' + cardNumber + '&cardSerial=' + cardSerial + '&cardValue=' + cardValue + '&nickName=' + nickName
            + '&telOperatorId=' + telOperatorId + '&fromDate=' + fromDate + '&toDate=' + toDate + '&currentPage='
            + currPage + '&status=' + status + '&PartnerID=' + PartnerID + '&smg=' + smg + '&serviceId=' + serviceId;
        if (PartnerErrorCode != null) {
            param += '&PartnerErrorCode=' + PartnerErrorCode;
        }

        $.ajax({
            type: "GET",
            url: GlobalHeader.Config.Root_Url + "Card/GetCardRechardList" + param,
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (html) {
                $('#hisCardRechard').html(html);
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }

    GlobalHeader.RefundToUser = function (requestId) {
        var inputData = { requestId: requestId };
        if (!confirm("Có muốn hoàn tiền cho khách hàng này?")) {
            return;
        }

        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Card/RefundToUser",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',

            success: function (data) {
                console.log(data)
                if (data.ResponseCode == 1) {
                    alert(data.Message);
                    GlobalHeader.getHistoryCardRechard(1);
                } else {
                    alert(data.Message);
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }

    GlobalHeader.RejectToUser = function (requestId) {
        var inputData = { requestId: requestId };
        if (!confirm("Từ chối thẻ nạp cho bản ghi này? ")) {
            return;
        }

        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Card/RejectToUser",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {
                console.log(data)
                if (data.ResponseCode == 1) {
                    alert(data.Message);
                    GlobalHeader.getHistoryCardRechard(1);
                } else {
                    alert(data.Message);
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }

    GlobalHeader.GetCardRechardById = function () {
        $requestId = $("#Check_RequestID").val();
        if ($requestId == null || $requestId.length <= 0) {
            alert("Chưa nhập mã yêu cầu")
        };
        var inputData = { requestId: $requestId };

        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Card/GetCardRechardById",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',

            success: function (data) {
                console.log(data)
                if (data.ResponseCode == 1) {
                    $("#Check_NickName").val(data.obj.DisplayName);
                    $("#Check_Tele").val(data.obj.OperatorName);
                    $("#Check_CardNumber").val(data.obj.CardNumber);

                    $("#Check_Serial").val(data.obj.SerialNumber);
                    $("#Check_Value").val(data.obj.CardValue);
                    $("#Check_Status").val(data.obj.StatusStr);

                } else {
                    alert(data.Message);
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }
    GlobalHeader.AddBackListTx = function () {
        var ok = confirm("Bạn muốn thêm tài khoản này vào danh sách backlist tài xỉu");
        if (!ok)
            return false;
        var $ServiceID = $("#ServiceIDAdd").val();
        var $DisplayName = $("#DisplayNameAdd").val();
        if ($ServiceID == null || $ServiceID.length <= 0) {
            alert("Cổng game không thể trống");
            return false;
        };
        if ($DisplayName == null || $DisplayName.length <= 0) {
            alert("Tên nhât nhật không thể trống")
            return false;
        };
        var inputData = { DisplayName: $DisplayName, ServiceID: $ServiceID };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "TxBackList/Create",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {
                alert(data.Message);
                if (data.Code == 1) {
                    $("#DisplayNameAdd").val('');
                }

                $('#btnSearch').click();
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }

    GlobalHeader.RemoveBackListTx = function (AccountID) {
        var ok = confirm("Bạn muốn xóa tài khoản này vào danh sách backlist tài xỉu");
        if (!ok)
            return false;
        var $AccountID = AccountID;

        if ($AccountID == null || $AccountID.length <= 0) {
            alert("AccountID không thể trống");
            return false;
        };

        var inputData = { AccountID: $AccountID };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "TxBackList/Delete",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {
                alert(data.Message);
                $('#btnSearch').click();
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }
    GlobalHeader.TransRefund = function (Id) {
        var ok = confirm("Thu hồi mã giao dịch" + Id);
        if (!ok)
            return false;
        if (Id == null || Id.length <= 0) {
            alert("Mã giao dịch không thể trống")
            return false;
        };

        var inputData = { Id: Id };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Admin/TransactionFreeExecute",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {
                alert(data.Message);
                $('#btnSearch').click();
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }
    GlobalHeader.TransRefundAgency = function (Id) {
        var ok = confirm("Thu hồi mã giao dịch" + Id);
        if (!ok)
            return false;
        if (Id == null || Id.length <= 0) {
            alert("Mã giao dịch không thể trống")
        };

        var inputData = { Id: Id };
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Admin/TransactionFreeExecuteAgency",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {
                alert(data.Message);
                $('#btnSearch').click();
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }
    GlobalHeader.NotDoAnyThing = function () {
        $requestId = $("#Check_RequestID").val();
        $Amount = $("#Check_Refund").val();
        if ($requestId == null || $requestId.length <= 0) {
            alert("Chưa nhập mã yêu cầu")
        };
        if ($Amount === null || $Amount.length <= 0) {
            alert("Mệnh giá chỉ chấp nhân 10.000,20.000")
        }
        if ($Amount <= 0) {
            alert("Mệnh giá chỉ chấp nhân 10.000,20.000")
        }
        var inputData = { requestId: $requestId, Amount: $Amount };

        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Card/NotDoAnyThing",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (data) {
                console.log(data)
                if (data.ResponseCode == 1) {
                    alert(data.Message);
                    $("#Check_NickName").val("");
                    $("#Check_Tele").val("");
                    $("#Check_CardNumber").val("");
                    $("#Check_Serial").val("");
                    $("#Check_Value").val("");
                    $("#Check_RequestID").val("");
                    $("#Check_Refund").val("");
                    GlobalHeader.getHistoryCardRechard(1);
                } else {
                    alert(data.Message);
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }

    GlobalHeader.RefundMoneyInValidCard = function () {
        $requestId = $("#Check_RequestID").val();
        $Amount = $("#Check_Refund").val();
        if ($requestId == null || $requestId.length <= 0) {
            alert("Chưa nhập mã yêu cầu")
        };
        if ($Amount === null || $Amount.length <= 0) {
            alert("Mệnh giá chỉ chấp nhân 50.000,100.000,200.000,500.000,1.000.000")
        }
        if ($Amount <= 0) {
            alert("Mệnh giá chỉ chấp nhân 50.000,100.000,200.000,500.000,1.000.000")
        }
        var inputData = { requestId: $requestId, Amount: $Amount };

        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "Card/RefundMoneyInValidCard",
            data: JSON.stringify(inputData),
            contentType: "application/json; charset=utf-8",
            dataType: 'json',

            success: function (data) {
                console.log(data)
                if (data.ResponseCode == 1) {
                    alert(data.Message);
                    $("#Check_NickName").val("");
                    $("#Check_Tele").val("");
                    $("#Check_CardNumber").val("");

                    $("#Check_Serial").val("");
                    $("#Check_Value").val("");
                    $("#Check_RequestID").val("");
                    $("#Check_Refund").val("");
                    GlobalHeader.getHistoryCardRechard(1);
                } else {
                    alert(data.Message);
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }
    GlobalHeader.validationSetGiftcode = function (type) {
        if (type == 0) {
            var campaignId = $("#campaignId").val().trim();
            var giftCode = $("#giftCode").val().trim();
            if (campaignId == 0) {
                GlobalHeader.dataValMsgFor('campaignId', 'Vui lòng chọn chiến dịch!')
                return false;
            }

            if (giftCode == '' || giftCode == null) {
                GlobalHeader.dataValMsgFor('giftCode', 'Vui lòng nhập giftcode!')
                $("#giftCode").focus();
                return false;
            }
        }

        var moneyReward = $("#moneyReward").val().trim();
        var totalUsed = $("#totalUsed").val().trim();
        var limitQuota = $("#limitQuota").val().trim();
        var expiredDate = $("#expiredDate").val().trim();
        var MAX_INT = Math.pow(2, 53);

        if (moneyReward == null || moneyReward == '') {
            GlobalHeader.dataValMsgFor('moneyReward', 'Vui lòng nhập tiền thưởng!')
            $("#moneyReward").focus();
            return false;
        } else if (moneyReward.replace(/\./g, '') < 1 || moneyReward.replace(/\./g, '') > MAX_INT) {
            GlobalHeader.dataValMsgFor('moneyReward', 'Giá trị nhập vào không hợp lệ!');
            $("#moneyReward").focus();
            return false;
        }

        if (totalUsed == null || totalUsed == '') {
            GlobalHeader.dataValMsgFor('totalUsed', 'Vui lòng nhập tổng người nhận!')
            $("#totalUsed").focus();
            return false;
        } else if (totalUsed.replace(/\./g, '') < 1 || totalUsed.replace(/\./g, '') > MAX_INT) {
            GlobalHeader.dataValMsgFor('totalUsed', 'Giá trị nhập vào không hợp lệ!');
            $("#totalUsed").focus();
            return false;
        }

        if (limitQuota == null || limitQuota == '') {
            GlobalHeader.dataValMsgFor('limitQuota', 'Vui lòng nhập giới hạn!')
            $("#limitQuota").focus();
            return false;
        } else if (limitQuota.replace(/\./g, '') < 1 || limitQuota.replace(/\./g, '') > MAX_INT) {
            GlobalHeader.dataValMsgFor('limitQuota', 'Giá trị nhập vào không hợp lệ!');
            $("#limitQuota").focus();
            return false;
        }

        if (expiredDate == '' || expiredDate == null) {
            GlobalHeader.dataValMsgFor('expiredDate', 'Vui lòng nhập ngày sự kiện!');
            $("#expiredDate").focus();
            return false;
        }

        return true;
    }

    GlobalHeader.validationArtifacts = function (type) {
        if (type == 0) {
            var artifactsCode = $("#artifactsCode").val().trim();
            if (artifactsCode == null || artifactsCode == '') {
                GlobalHeader.dataValMsgFor('artifactsCode', 'Vui lòng nhập mã hiện vật!')
                $("#artifactsCode").focus();
                return false;
            }
        }

        var artifactsName = $("#artifactsName").val().trim();
        var price = $("#price").val().trim();
        var description = $("#description").val().trim();
        var MAX_INT = Math.pow(2, 53);

        if (artifactsName == null || artifactsName == '') {
            GlobalHeader.dataValMsgFor('artifactsName', 'Vui lòng nhập tên hiện vật!')
            $("#artifactsName").focus();
            return false;
        }

        if (price == '' || price == null) {
            GlobalHeader.dataValMsgFor('price', 'Vui lòng nhập giá!');
            $("#price").focus();
            return false;
        } else if (price.replace(/\./g, '') < 1 || price.replace(/\./g, '') > MAX_INT) {
            GlobalHeader.dataValMsgFor('price', 'Giá nhập vào không hợp lệ!');
            $("#price").focus();
            return false;
        }

        if (description == '') {
            GlobalHeader.dataValMsgFor('description', 'Vui lòng nhập miêu tả!');
            $("#description").focus();
            return false;
        }

        return true;
    };


    GlobalHeader.validationPrivilegeArtifacts = function (type) {
        if (type == 0) {
            var privilegeId = $("#privilegeId").val().trim();
            var artifactId = $("#artifactId").val().trim();
            if (privilegeId < 1 || privilegeId > 5) {
                GlobalHeader.dataValMsgFor('privilegeId', 'Hạng không hợp lệ!')
                return false;
            }

            if (artifactId < 1) {
                GlobalHeader.dataValMsgFor('artifactId', 'Tên hiện vật không hợp lệ!')
                return false;
            }
        }
        var quantity = $("#quantity").val().trim();
        var description = $("#description").val().trim();
        var MAX_INT = Math.pow(2, 32);

        if (quantity == '' || quantity == null) {
            GlobalHeader.dataValMsgFor('quantity', 'Vui lòng nhập số lượng giải!');
            $("#quantity").focus();
            return false;
        } else if (quantity.replace(/\./g, '') < 1 || quantity.replace(/\./g, '') > MAX_INT) {
            GlobalHeader.dataValMsgFor('quantity', 'Số lượng giải nhập vào không hợp lệ!');
            $("#quantity").focus();
            return false;
        }

        if (description == '') {
            GlobalHeader.dataValMsgFor('description', 'Vui lòng nhập miêu tả!');
            $("#description").focus();
            return false;
        }

        return true;
    };

    GlobalHeader.agcGetRaceTop = function (race, from, to) {
        $.ajax({
            type: "GET",
            url: GlobalHeader.Config.Root_Url + "Agency/GetRaceTop?raceDate=" + race + "&fromDate=" + from + "&toDate=" + to,
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (html) {
                $('#agc-racetop-content').html(html);
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    }

    GlobalHeader.dataValMsgFor = function (name, msg) {
        $('.adminData [data-valmsg-for=' + name + ']').removeClass('field-validation-valid').addClass("field-validation-error").html(msg);
        setTimeout(function () {
            $('.adminData [data-valmsg-for=' + name + ']').removeClass('field-validation-error').html('').addClass("field-validation-valid");
        }, 3000);
    };

    GlobalHeader.onchangeArtifacts = function () {
        var optVal = $("#artifactId option:selected").val();
        if (optVal == 0) {
            $('#price').val('');
            $('#totalPrize').val('');
        } else {
            $.ajax({
                type: "GET",
                url: GlobalHeader.Config.Root_Url + "Manager/GetArtifactsPrice/" + optVal,
                success: function (data) {
                    $('#price').val(data);
                    var quantity = $('#quantity').val().trim().replace(/\./g, '');
                    var totalPrize = quantity * data.replace(/\./g, '');
                    $('#totalPrize').val(GlobalHeader.formatNumber(totalPrize));
                },
                fail: function (fail) {
                    console.log('loi')
                }
            });
        }
    };

    GlobalHeader.bindGiftcodeProgress = function () {
        var nickName = $("#nickNameProgress").val().trim();
        var accountType = $("#accountTypeProgress").val().trim();
        var ServiceID = $("#serviceIdReport").val().trim();

        $.ajax({
            type: "GET",
            url: GlobalHeader.Config.Root_Url + "GiftCode/GetGiftcodeProgress?nickName=" + nickName + "&accountType=" + accountType + "&ServiceID=" + ServiceID,
            success: function (html) {
                $('#bindGiftcodeProgress').html(html);
            },
            fail: function (fail) {
                console.log('loi')
            }
        });
    };

    GlobalHeader.bindDataTransferAmount = function () {
        var amount = $('#amount').val().trim().replace(/\./g, '');
        if (amount == null || amount == '')
            amount = 0;
        var transfee = $('#transfee').val().trim();
        var fee = amount * transfee;
        var orgAmount = parseInt(amount) + parseInt(fee);
        $('#orgAmount').val(GlobalHeader.formatNumber(orgAmount));
    }

    GlobalHeader.quantityArtifacts = function (e, quantityOld) {
        var quantity = parseInt(e.value.replace(/\./g, ''));
        var price = $('#price').val().replace(/\./g, '');
        var remain = parseInt($('#remainQuantity').val().replace(/\./g, '')) || 0;
        var totalPrize = quantity * price;
        var remainNew = parseInt(remain + quantity - quantityOld);
        $('#remainQuantity').val(GlobalHeader.formatNumber(remainNew));
        $('#totalPrize').val(GlobalHeader.formatNumber(totalPrize));
    }

    GlobalHeader.isNumber = function (evt) {
        evt = (evt) ? evt : window.event;
        var charCode = (evt.which) ? evt.which : evt.keyCode;
        var backSpace = evt.Key;

        if (charCode > 31 && (charCode < 48 || charCode > 57)) {
            return false;
        }

        return true;
    }

    GlobalHeader.changeNumber = function (e) {
        e.value = e.value.replace(/[^0-9\.]/g, '');
        e.value = e.value.replace(/\./g, '');
        e.value = GlobalHeader.formatNumber(e.value);
    };

    GlobalHeader.formatNumber = function (number) {
        number += '';
        x = number.split(',');
        x1 = x[0];
        x2 = x.length > 1 ? ',' + x[1] : '';
        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1))
            x1 = x1.replace(rgx, '$1' + '.' + '$2');

        return x1 + x2;
    };

    GlobalHeader.getDateNow = function () {
        var d = new Date();
        var _day = d.getDate();
        var _month = d.getMonth() + 1;
        var _year = d.getFullYear();
        if (_day < 10) _day = "0" + _day;
        if (_month < 10) _month = "0" + _month;

        return _year + '' + _month + '' + _day;
    }

    GlobalHeader.formatDateTimehms = function (date, option) {
        if (typeof (option) == 'undefined') option = 0;
        date = date.replace(/\-/g, '\/').replace(/[T|Z]/g, ' ');
        if (date.indexOf('.') > 0) date = date.substring(0, date.indexOf('.'));
        var d = new Date(date);
        var _day = d.getDate();
        var _month = d.getMonth() + 1;
        var _year = d.getFullYear();
        var _hour = d.getHours();
        var _minute = d.getMinutes();
        var _second = d.getSeconds();
        if (_day < 10) _day = "0" + _day;
        if (_month < 10) _month = "0" + _month;
        if (_hour < 10) _hour = "0" + _hour;
        if (_minute < 10) _minute = "0" + _minute;
        if (_second < 10) _second = "0" + _second;

        switch (option) {
            case 0:
                return _day + '/' + _month + '/' + _year + ' ' + _hour + ':' + _minute;
                break;
            case 1:
                return _day + '/' + _month + ' ' + _hour + ':' + _minute;
                break;
            case 2:
                return _hour + ':' + _minute + ':' + _second + ' ' + _day + '/' + _month + '/' + _year;
                break;
            case 3:
                return _year + '/' + _month + '/' + _day + ' ' + _hour + ':' + _minute + ':' + _second;
                break;
            case 4:
                return _day + '/' + _month + '/' + _year;
                break;
            case 5:
                return _hour + ':' + _minute + ':' + _second;
                break;
            case 6:
                return _hour + ':' + _minute;
                break;
            case 7:
                return _year + '' + _month + '' + _day;
                break;
            default:
                return date.toString();
                break;
        };
    }


    GlobalHeader.inputKeypress = function (keycode) {
        if (keycode == 13) {
            $('#btnSearch').click();
        }
    };

    GlobalHeader.notifyMessage = function (mes, type) {
        $('#text-notifymsg').text(mes);
        if (type == 1) {
            $('#text-notifymsg').addClass("txttransferMsgSuccess");
        } else {
            $('#text-notifymsg').addClass("txttransferMsg");
        }
        setTimeout(function () {
            $('#text-notifymsg').text('');
        }, 3000);
    };

    GlobalHeader.errorMessage = function (mes) {
        $('.rowtext-errmsg').show();
        $('.text-errmsg').html(mes);
        setTimeout(function () {
            $('.text-errmsg').html('');
            $('.rowtext-errmsg').hide();
        }, 2000);
    };

    GlobalHeader.getAccountGameProfit = function () {
        //get gameInfo
        $.ajax({
            type: "GET",
            url: GlobalHeader.Config.Root_Url + "CustomerSupport/GetGameInfo",
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (data) {
                var parseData = JSON.parse(data);
                var l = parseData.length;
                var totalBetValue = 0;
                var totalPrizeValue = 0;

                for (var i = 0; i < l; i++) {
                    $.ajax({
                        type: "GET",
                        url: GlobalHeader.Config.Root_Url + "CustomerSupport/AccountGameProfit?gameId=" + parseData[i].Value,
                        success: function (result) {
                            if (result != null && result != '') {
                                var parseResult = JSON.parse(result);
                                totalBetValue += parseResult.TotalBetValue;
                                totalPrizeValue += parseResult.TotalPrizeValue;
                                var html = '';
                                html += '<tr>';
                                html += '<td rowspan="2">' + parseResult.GameName + '</td>';
                                html += '<td>Tổng đặt - thưởng</td>';
                                html += '<td>' + GlobalHeader.formatNumber(parseResult.TotalBetValue) + ' - ' + GlobalHeader.formatNumber(parseResult.TotalPrizeValue) + '</td>';
                                html += '</tr>';
                                html += '<tr style="background-color: #c0c0c0;">';
                                html += '<td>Tổng đặt/thưởng</td>';
                                html += '<td>' + GlobalHeader.formatNumber(parseResult.RateBetPrize) + '</td>';
                                html += '</tr>';
                                $('#accountgame-profit').append(html);
                                $('#profit-totalbet').text(GlobalHeader.formatNumber(totalBetValue));
                                $('#profit-totalprize').text(GlobalHeader.formatNumber(totalPrizeValue));
                                $('#profit-totalprofit').text(GlobalHeader.formatNumber(totalPrizeValue - totalBetValue));
                            }
                        },
                        fail: function (fail) {
                            console.log("Loi");
                        }
                    });
                }
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };

    GlobalHeader.getAccountLoginIP = function (accountId, top) {
        if (accountId < 1 || top < 1)
            return;

        $.ajax({
            type: "GET",
            url: GlobalHeader.Config.Root_Url + "CustomerSupport/AccountLoginIP?accountId=" + accountId + "&top=" + top,
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (html) {
                $('#lstIpLoginModal .modal-body').html(html);
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };
    GlobalHeader.getFormConfigLive = function (accountId, top) {
        if (accountId < 1 || top < 1)
            return;
        $('#lstConfigLiveModal .modal-body').empty().html("Đang lấy thông tin cấu hình!");
        $.ajax({
            type: "GET",
            url: GlobalHeader.Config.Root_Url + "CustomerSupport/ConfigLiveUser?accountId=" + accountId + "&top=" + top,
            xhrFields: { withCredentials: true },
            crossDomain: true,
            success: function (html) {
                $('#lstConfigLiveModal .modal-body').html(html);
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };
    GlobalHeader.submitFormConfigLive = function (accountId, top) {
        if (accountId < 1 || top < 1)
            return;
        var price = $("input[name='price[]']:checked").map(function () {
            return this.value;
        }).get();
        var game = $("input[name='game[]']:checked").map(function () {
            return this.value;
        }).get();
        console.log({
            Price: price,
            Game: game
        });
        $.ajax({
            type: "POST",
            url: GlobalHeader.Config.Root_Url + "CustomerSupport/ConfigLiveUser?accountId=" + accountId + "&top=" + top,
            xhrFields: { withCredentials: true },
            crossDomain: true,
            data: {
                UserId: $("#accountConfigLive").val(),
                Tranfer: price,
                Game: game
            },
            success: function (html) {
                console.log(html);
            },
            fail: function (fail) {
                console.log("Loi");
            }
        });
    };
})();